namespace ZombieDoorBreaking {
    public static class Extensions {

        public static void RAMessage( this CommandSender sender, string message, bool success = true ) =>
            sender.RaReply("<color=green>ZDB</color>#" + message, success, true, string.Empty);

        public static void Broadcast( this ReferenceHub rh, uint time, string message ) => 
            rh.GetComponent<Broadcast>().TargetAddElement(rh.scp079PlayerScript.connectionToClient, message, time, false);

        public static bool EqualsIgnoreCase(this string sryLemonIloveJava, string to) =>
            sryLemonIloveJava.ToLower().Equals(to.ToLower());
    }
}