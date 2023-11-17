-- Functions to perform moves on the cube
local CubeMove = {}
CubeMove.__index = CubeMove

function CubeMove:new(size, faces)
    return setmetatable({
        size = size,
        faces = faces
    }, CubeMove)
end

-- Returns a cubes size
function CubeMove:getSize()
    return self.size
end

-- Returns a cubes faces
function CubeMove:getFaces()
    return self.faces
end

-- Sets a cubes size
function CubeMove:setSize(size)
    self.size = size
end

-- Sets a cubes faces
function CubeMove:setFaces(faces)
    self.faces = faces
end

-- Rotate a cube face
function CubeMove:rotateFace(face, direction)
    if not direction then
        face:rotate_cw()
    else
        face:rotate_ccw()
    end
end

-- Move a cube layer on the X axis
function CubeMove:moveXAxis(layer, direction)
    -- Layers L -> R = 1 -> 3

    local mirrored_layer = (self.size+1) - layer
    local org = {
        F = self.faces.F:getColumn(layer),
        U = self.faces.U:getColumn(layer),
        B = self.faces.B:getColumn(mirrored_layer),
        D = self.faces.D:getColumn(layer),
    }

    if not direction then
        -- CW
        self.faces.F:setColumn(layer, org.D)
        self.faces.U:setColumn(layer, org.F)
        self.faces.B:setColumnReversed(mirrored_layer, org.U)
        self.faces.D:setColumnReversed(layer, org.B)
    else
        -- CCW
        self.faces.F:setColumn(layer, org.U)
        self.faces.U:setColumnReversed(layer, org.B)
        self.faces.B:setColumnReversed(mirrored_layer, org.D)
        self.faces.D:setColumn(layer, org.F)
    end
end

-- Move a cube layer on the Y axis
function CubeMove:moveYAxis(layer, direction)
    -- Layers U -> D = 1 -> 3

    local org = {
        L = self.faces.L:getRow(layer),
        F = self.faces.F:getRow(layer),
        R = self.faces.R:getRow(layer),
        B = self.faces.B:getRow(layer),
    }

    if not direction then
        -- CW
        self.faces.L:setRow(layer, org.F)
        self.faces.F:setRow(layer, org.R)
        self.faces.R:setRow(layer, org.B)
        self.faces.B:setRow(layer, org.L)
    else
        -- CCW
        self.faces.L:setRow(layer, org.B)
        self.faces.F:setRow(layer, org.L)
        self.faces.R:setRow(layer, org.F)
        self.faces.B:setRow(layer, org.R)
    end
end

-- Move a cube layer on the Z axis
function CubeMove:moveZAxis(layer, direction)
    -- Layers F -> B = 1 -> 3

    local mirrored_layer = (self.size+1) - layer
    local org = {
        U = self.faces.U:getRow(mirrored_layer),
        R = self.faces.R:getColumn(layer),
        D = self.faces.D:getRow(layer),
        L = self.faces.L:getColumn(mirrored_layer),
    }

    if not direction then
        -- CW
        self.faces.U:setRowReversed(mirrored_layer, org.L)
        self.faces.R:setColumn(layer, org.U)
        self.faces.D:setRowReversed(layer, org.R)
        self.faces.L:setColumn(mirrored_layer, org.D)
    else
        -- CCW
        self.faces.U:setRow(mirrored_layer, org.R)
        self.faces.R:setColumnReversed(layer, org.D)
        self.faces.D:setRow(layer, org.L)
        self.faces.L:setColumnReversed(mirrored_layer, org.U)
    end
end

-- Perform a cube rotation on the X axis
function CubeMove:rotateXAxis(direction)
    for layer = 1, self.size do
        self:moveXAxis(layer, direction)
    end
    self:rotateFace(self.faces.R, direction)
    self:rotateFace(self.faces.L, not direction)
end

-- Perform a cube rotation on the Y axis
function CubeMove:rotateYAxis(direction)
    for layer = 1, self.size do
        self:moveYAxis(layer, direction)
    end
    self:rotateFace(self.faces.U, direction)
    self:rotateFace(self.faces.D, not direction)
end

-- Perform a cube rotation on the Z axis
function CubeMove:rotateZAxis(direction)
    for layer = 1, self.size do
        self:moveZAxis(layer, direction)
    end
    self:rotateFace(self.faces.F, direction)
    self:rotateFace(self.faces.B, not direction)
end

-- Move a cube layer by a given axis [ X=1, Y=2, Z=3 ]
function CubeMove:moveByAxis(axis, layer, direction)
    if axis == 1 then
        self:moveXAxis(layer, direction)
    elseif axis == 2 then
        self:moveYAxis(layer, direction)
    elseif axis == 3 then
        self:moveZAxis(layer, direction)
    else
        error('Invalid axis: ' .. axis)
    end
end

-- Rotate a cube by a given axis [ X=1, Y=2, Z=3 ]
function CubeMove:rotateByAxis(axis, direction)
    if axis == 1 then
        self:rotateXAxis(direction)
    elseif axis == 2 then
        self:rotateYAxis(direction)
    elseif axis == 3 then
        self:rotateZAxis(direction)
    else
        error('Invalid axis: ' .. axis)
    end
end

return CubeMove