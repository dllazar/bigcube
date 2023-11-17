local CubePrint = require("Lubiks.CubePrint")

local CubeFace = {}
CubeFace.__index = CubeFace

-- Creates a cube face
function CubeFace:new(size, color)
    local face = {}
    self.size = size
    for i = 1, size do
        local row = {}
        for j = 1, size do
            row[j] = color
        end
        face[i] = row
    end

    return setmetatable(face, CubeFace)
end

function CubeFace:_reverse(list)
    local reversed = {}
    for i = #list, 1, -1 do
        reversed[#reversed+1] = list[i]
    end
    return reversed
end

-- Get/Set cube colors in row
function CubeFace:getRow(row_index)
    return self[row_index]
end

function CubeFace:getRowReversed(row_index)
    local row = self:getRow(row_index)
    return self:_reverse(row)
end

function CubeFace:setRow(row_index, row_list)
    self[row_index] = row_list
end

function CubeFace:setRowReversed(row_index, row_list)
    row_list = self:_reverse(row_list)
    self:setRow(row_index, row_list)
end

-- Get/Set cube colors in column
function CubeFace:getColumn(column_index)
    local column = {}
    for row = 1, #self do
        column[row] = self[row][column_index]
    end
    return column
end

function CubeFace:getColumnReversed(column_index)
    local column = self:getColumn(column_index)
    return self:_reverse(column)
end

function CubeFace:setColumn(column_index, column_list)
    for row = 1, #self do
        self[row][column_index] = column_list[row]
    end
end

function CubeFace:setColumnReversed(column_index, column_list)
    column_list = self:_reverse(column_list)
    self:setColumn(column_index, column_list)
end

-- Rotate cube face
function CubeFace:rotate_cw() -- Rotate face clockwise
    local rotated = {}

    for i = 1, #self[1] do
        rotated[i] = {}
        for j = 1, #self do
            rotated[i][j] = self[#self-j+1][i]
        end
    end

    for i = 1, #self do
        self[i] = rotated[i]
    end
end

function CubeFace:rotate_ccw() -- Rotate face counter-clockwise
    local rotated = {}

    for i = #self[1], 1, -1 do
        rotated[#self[1]-i+1] = {}
        for j = 1, #self do
            rotated[#self[1]-i+1][j] = self[j][i]
        end
    end

    for i = 1, #self do
        self[i] = rotated[i]
    end
end

return CubeFace