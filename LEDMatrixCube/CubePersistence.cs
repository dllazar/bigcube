using RubiksCube;

namespace LEDMatrixCube
{
    internal static class CubePersistence
    {
        public static void SaveCubeState(Cube cube, string filePath = ".cubestate")
        {
            using var writer = new StreamWriter(path: filePath, append: false);
            var state = cube.GetFaces();
            var json = System.Text.Json.JsonSerializer.Serialize(state);
            writer.Write(json);
            writer.Flush();
        }
        public static void LoadIfSaveFileExists(Cube cube, string filePath = ".cubestate")
        {
            if (File.Exists(filePath))
            {
                using var reader = new StreamReader(path: filePath);
                var json = reader.ReadToEnd();
                var state = System.Text.Json.JsonSerializer.Deserialize<Dictionary<Face, string>>(json);
                if (state != null)
                {
                    cube.SetFaces(state); 
                }
            }
        }
    }
}
