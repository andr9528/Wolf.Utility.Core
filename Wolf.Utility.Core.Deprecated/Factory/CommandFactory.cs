using System;

using Wolf.Utility.Core.Deprecated.Factory.Enum;

namespace Wolf.Utility.Core.Factory
{
    public class CommandFactory
    {
        private static CommandFactory instance;

        public static CommandFactory Instance()
        {
            if (instance == null) instance = new CommandFactory();
            return instance;
        }

        #region Byte Commands
        byte[] SELECT = {
            (byte) 0x00, // CLA Class           
            (byte) 0xA4, // INS Instruction     
            (byte) 0x04, // P1  Parameter 1
            (byte) 0x00, // P2  Parameter 2
            (byte) 0x0A, // Length
            0x63,0x64,0x63,0x00,0x00,0x00,0x00,0x32,0x32,0x31 // AID
        };

        byte[] GET_STRING = {
            (byte) 0x80, // CLA Class        
            0x04, // INS Instruction
            0x00, // P1  Parameter 1
            0x00, // P2  Parameter 2
            0x10  // LE  maximal number of bytes expected in result
        };
        #endregion

        public byte[] GetByteCommand(NfcByteCommand command)
        {
            switch (command)
            {
                case NfcByteCommand.Select:
                    return SELECT;
                case NfcByteCommand.GetString:
                    return GET_STRING;
                default:
                    throw new ArgumentOutOfRangeException(nameof(command), command, "You requested a command that has no matching Byte[] - CommandFactory");
            }
        }
    }
}