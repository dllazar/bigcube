-- To print 2d cube
local CubePrint = {}
CubePrint.__index = CubePrint

-- Print a cube as a 2d string
function CubePrint:Print(cube)
    local string_cube = self:toString(cube)
    print(string_cube)
end

-- Convert a cube to a 2d string
function CubePrint:toString(cube)
    local faces_index = {"U", "L", "F", "R", "B", "D"}

    local middle_cube = {}
    for i = 1, cube:getSize() do
        local faces_str = {}
        for j = 2, 5 do
            local face = cube:getFace(faces_index[j])[i]
            faces_str[#faces_str+1] = table.concat(face, " ")
        end
        middle_cube[i] = table.concat(faces_str, " ")
    end

    local cube_str_buff = {}
    local tab = (" "):rep(cube:getSize()*2)
    table.insert(cube_str_buff, self:faceToString(cube:getFace("U"), tab))
    table.insert(cube_str_buff, table.concat(middle_cube, "\n"))
    table.insert(cube_str_buff, self:faceToString(cube:getFace("D"), tab))

    return table.concat(cube_str_buff, "\n")
end

-- Convert a cube face to a string
function CubePrint:faceToString(face, tab)
    local face_str_buff = {}
    for i = 1, #face do
        face_str_buff[i] = (tab .. table.concat(face[i], " "))
    end
    return table.concat(face_str_buff, "\n")
end

return CubePrint