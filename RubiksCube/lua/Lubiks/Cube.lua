-- Lua Simple NxNxN Rubiks Cube Implementation
-- 28.3.2023

local CubeFace = require("Lubiks.CubeFace")
local CubeMove = require("Lubiks.CubeMove")
local CubePrint = require("Lubiks.CubePrint")

local Cube = {}
Cube.__index = Cube

function Cube.__tostring(self)
    return CubePrint:toString(self)
end

function Cube:new(size)
    size = size or 3

    local new = setmetatable({
        MoveHistory = {},
        CubeMove = CubeMove:new(),
    }, Cube)

    new:setSize(size)
    new:reset()
    return new
end

-- Return the cubes size
function Cube:getSize()
    return self.size
end

-- Return the cubes faces
function Cube:getFaces()
    return self.faces
end

-- Return a cube face
function Cube:getFace(face)
    return self.faces[face]
end

-- Return cube move history
function Cube:getMoveHistory()
    return self.MoveHistory
end

-- Clears cube move history
function Cube:clearMoveHistory()
    self.MoveHistory = {}
end

-- Set the cubes size [ NxNxN ]
function Cube:setSize(size)
    if size <= 1 then
        error("Invalid cube size: " .. size)
    end

    self.size = size
    self.CubeMove:setSize(self.size)
    self:reset() -- reset on resize
end

-- Set a faces state
function Cube:setFace(face, state)
    if #state ~= self.size then
        error("Invalid face size: " .. #state)
    end

    for row = 1, #state do
        self.faces[face]:setRow(row, state[row])
    end
end

-- Resets the cube into its initial state
function Cube:reset()
    --[[
          W W W
          W W W
          W W W
    O O O G G G R R R B B B
    O O O G G G R R R B B B
    O O O G G G R R R B B B
          Y Y Y
          Y Y Y
          Y Y Y
    ]]

    self.MoveHistory = {}
    self.faces = {
        U = CubeFace:new(self.size, 'W'), -- White
        L = CubeFace:new(self.size, 'O'), -- Orange
        F = CubeFace:new(self.size, 'G'), -- Green
        R = CubeFace:new(self.size, 'R'), -- Red
        B = CubeFace:new(self.size, 'B'), -- Blue
        D = CubeFace:new(self.size, 'Y'), -- Yellow
    }

    self.CubeMove:setFaces(self.faces)
end

-- Determine if the cube is solved
function Cube:solved(face_colors)
    local face_colors = face_colors or {
        U = 'W',
        L = 'O',
        F = 'G',
        R = 'R',
        B = 'B',
        D = 'Y',
    }

    for face_name, face in pairs(self.faces) do
        local face_color = face_colors[face_name]
        for _, row in pairs(face) do
            for _, color in pairs(row) do
                if color ~= face_color then
                    return false
                end
            end
        end
    end
    return true
end

-- Generate a scramble string
function Cube:generateScramble(len, return_list)
    local scramble = {}
    local possible_moves = {
        'R', 'U', 'F',
        'L', 'D', 'B',
        -- 'M', 'E', 'S',
        -- 'X', 'Y', 'Z',
    }

    local last_two_moves = {'', ''}
    local allow_layer_move = (self.size > 5)
    local allow_wide_move  = (self.size > 3)

    for i = 1, len do
        local move
        local layer = ''

        -- no repeating/undo moves [ hopefully ]
        while true do
            move = possible_moves[math.random(#possible_moves)]
            local was_last_two_moves = false

            for _, last_move in pairs(last_two_moves) do
                if last_move == move then
                    was_last_two_moves = true
                end
            end

            if not was_last_two_moves then
                break
            end
        end

        table.insert(last_two_moves, 1, move)
        table.remove(last_two_moves)

        -- layer moves
        if allow_layer_move and math.random(2) == 2 then
            layer = tostring(math.random(2, math.floor(self.size / 2) +1))
        end

        -- wide
        if allow_wide_move and (layer ~= '' or math.random(2) == 2) then
            move = (move .. 'w')
        end

        scramble[i] = ("%s%s%s"):format(layer, move, ({'', "'", "2"})[math.random(3)])
    end

    return return_list and scramble or table.concat(scramble, ' ')
end

-- Undo an amount of moves
function Cube:undoMove(count)
    count = count or 1
    for i = #self.MoveHistory, (#self.MoveHistory-count+1), -1 do
        local move = self.MoveHistory[i]
        if not move then break end

        local isPrime = (move:sub(#move, #move) == "'")
        move = (isPrime and move:sub(1,-2) or move .. "'") -- flip direction
        self:moveOnce(move, true)

        self.MoveHistory[i] = nil
    end
end

-- Perform a move on the cube [e.g.: "R U R' U'"]
function Cube:move(movements)
    for move in movements:gmatch("[^%s]+") do
        self:moveOnce(move)
    end
end

-- Move the cube once [e.g.: R, x2, 2r4]
function Cube:moveOnce(move, blockHistory)
    local parsed = self:_parseMove(move)

    -- append move
    if not blockHistory then
        self.MoveHistory[#self.MoveHistory+1] = move
    end

    if parsed.move_type == 3 or parsed.move_type == 4 then
        self:_moveXYZMES(parsed)
    else
        self:_moveULFRBD(parsed)
    end
end

-- Perform XYZMES moves
function Cube:_moveXYZMES(parsed)
    local middle = (math.floor(self.size / 2) +1)
    for _ = 1, parsed.times do
        if parsed.move_type == 4 then
            self.CubeMove:rotateByAxis(parsed.axis, parsed.direction) -- Cube rotation
        else
            self.CubeMove:moveByAxis(parsed.axis, middle, parsed.direction) -- Slice move
        end
    end
end

-- Perform ULFRBD moves
function Cube:_moveULFRBD(parsed)
    local correct_layer = self:_getCorrectLayer(parsed.layer, parsed.move_layer, parsed.wide) -- correct layer
    local step = (correct_layer > parsed.move_layer and -1 or 1)

    for layer = correct_layer, parsed.move_layer, step do
        for _ = 1, parsed.times do
            self.CubeMove:moveByAxis(parsed.axis, layer, parsed.direction) -- Move
            if layer == 1 or layer == parsed.move_layer then
                local face = self:getFace(parsed.move)
                self.CubeMove:rotateFace(face, parsed.prime) -- Rotate
            end
        end

        if not parsed.wide then
            break -- not wide, only move this layer
        end
    end
end

function Cube:_getMoveDirection(move) -- for determining cw/ccw
    local directions = {
        R = false, U = false, F = false,
        L = true,  D = true,  B = true,
        M = true,  E = true,  S = false,
        X = false, Y = false, Z = false,
    }
    return directions[move]
end

function Cube:_getMoveAxis(move)
    local axes = {
        R = 1, L = 1, M = 1, X = 1, -- X Axis
        U = 2, D = 2, E = 2, Y = 2, -- Y Axis
        F = 3, B = 3, S = 3, Z = 3, -- Z Axis
    }
    return axes[move]
end

function Cube:_getMoveType(move) -- similar to getMoveDirection
    local move_types = {
        R = 1, U = 1, F = 1,
        L = 2, D = 2, B = 2,
        M = 3, E = 3, S = 3,
        X = 4, Y = 4, Z = 4,
    }
    return move_types[move]
end

function Cube:_getMoveLayer(move)
    -- R moves CW, but L is index 1 so
    local layers = {
        L = 1,         U = 1,         F = 1,
        R = self.size, D = self.size, B = self.size,
    }
    return layers[move]
end

function Cube:_getCorrectLayer(layer, move_layer, wide) -- to correct layer moves
    local correct_layer = (layer or move_layer)
    local is_last_layer_move = (move_layer == self.size)

    if layer and is_last_layer_move then -- layer move
        correct_layer = (self.size+1) - layer
    elseif not layer and wide then -- normal wide move
        correct_layer = move_layer + (is_last_layer_move and -1 or 1)
    end

    return correct_layer
end

function Cube:_validateParsedMove(parsed, move_str) -- Move error checker
    if parsed.move == "" or
        (parsed.move_type == 3 or parsed.move_type == 4) and parsed.layer or -- rotation/slice by specific layer
        parsed.move_type == 3 and (self.size % 2 == 0) -- even cube doing M/E/S
    then
        error("Invalid cube move: " .. move_str)
    end
end

function Cube:_parseMove(move_str) -- Convert move to a parsed table
    local matched = {move_str:match("^%(?([0-9]*)([ULFRBDulfrbdXYZMESxyzmes])(w?)([0-9]*)('?)%)?$")}

    local parsed = {}
    parsed.move = (matched[2] or ''):upper()
    parsed.prime = (matched[5] == "'")
    parsed.times = (tonumber(matched[4]) or 1)

    -- Layers
    parsed.wide = (matched[3] == "w" or parsed.move:lower() == matched[2])
    parsed.layer = tonumber(matched[1])
    parsed.move_layer = self:_getMoveLayer(parsed.move)

    -- Moving components
    parsed.direction = (self:_getMoveDirection(parsed.move) ~= parsed.prime)
    parsed.axis = self:_getMoveAxis(parsed.move)
    parsed.move_type = self:_getMoveType(parsed.move)

    self:_validateParsedMove(parsed, parsed.move)

    return parsed
end

return Cube