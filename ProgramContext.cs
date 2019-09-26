namespace NoZ.Platform.IOS {
    public class ProgramContext {
        public string[] Args { get; set; }
        public string Name { get; set; }
        public string GameResourceName { get; set; }
        public ResourceArchive[] Archives { get; set; }
    }
}