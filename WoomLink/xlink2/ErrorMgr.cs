using System;
using System.Threading;

namespace WoomLink.xlink2
{
    public class ErrorMgr
    {
        public byte Field0 = 1;
        public System System;
        public Error[] Entries = new Error[32];
        public int ErrorDispFrame;
        public UintPointer Field5010;
        public int[] SeverityToUnk = new int[4];
        public int[] ErrorTypeToUnk = new int[40];
        private Mutex Mutex = new();

        public ErrorMgr(System system)
        {
            System = system;
        }

        public void Add(Error.Type type, User.User? user, string message)
        {
            var printedMessage = message;
            if (user != null)
                printedMessage = $"{user.Name} | {message}";
            Console.WriteLine($"ErrorMgr | {type} | {printedMessage}");
        }

        public void Clear(User.User user) { throw new NotImplementedException(); }

        public void ClearAll()
        {
            if (Field0 == 0)
                return;

            Mutex.WaitOne();

            for (var i = 0; i < Entries.Length; i++)
            {
                Entries[i].ErrorType = Error.Type.None;
            }

            Mutex.ReleaseMutex();
        }

        public void Calc() { throw new NotImplementedException(); }

        public void Draw(sead.TextWriter text) { throw new NotImplementedException(); }

        private bool ShouldErrorNoticed(in Error error)
        {
            /* Null check. */

            return ShouldErrorNoticed(error.ErrorType);
        }
        private bool ShouldErrorNoticed(Error.Type type)
        {
            var unk1 = SeverityToUnk[(int)TypeToSeverity(type)];
            var unk2 = ErrorTypeToUnk[(int)type];
            if (unk1 <= unk2)
                unk2 = unk1;
            return unk2 == 2;
        }

        public static Severity TypeToSeverity(Error.Type type)
        {
            var integer = (int)type;

            if (integer < (int)Error.Type.EventPoolFull)
                return Severity.Low;
            if (integer < (int)Error.Type.NotFoundEmitterSet)
                return Severity.Mid;

            return Severity.High;
        }

        /* Inferred? */
        public enum Severity
        {
            Low = 0,
            Mid = 1,
            High = 2,
        }
    }
}
