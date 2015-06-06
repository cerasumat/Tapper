using System;

namespace PCITC.MES.MM.Tapper.Framework.Utilities
{
    public static class DynamicCall
    {
        //======================================================================================================
        //object container = new Example();
        //decimal result = container.Dynamic<Guid, int, decimal>("HelloWorld", Guid.NewGuid(), 2009);
        //public class Example()
        //{
        //    public decimal HelloWorld(Guid userId, int year)
        //    {
        //        return userId.GetHashCode() + year;
        //    }
        //}
        //======================================================================================================
        public static TResult Dynamic<T, TResult>(this object container, string methodName, T arg)
        {
            return
                (TResult) Delegate.CreateDelegate(typeof (Func<T, TResult>), container, methodName).DynamicInvoke(arg);
        }

        public static TResult Dynamic<T1, T2, TResult>(this object container, string methodName, T1 arg1, T2 arg2)
        {
            return (TResult)Delegate.CreateDelegate(typeof(Func<T1, T2, TResult>), container, methodName).DynamicInvoke(arg1, arg2);
        }

        public static TResult Dynamic<T1, T2, T3, TResult>(this object container, string methodName, T1 arg1, T2 arg2, T3 arg3)
        {
            return (TResult)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, TResult>), container, methodName).DynamicInvoke(arg1, arg2, arg3);
        }

        public static TResult Dynamic<T1, T2, T3, T4, TResult>(this object container, string methodName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return (TResult)Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, TResult>), container, methodName).DynamicInvoke(arg1, arg2, arg3, arg4);
        }

        public static TResult Dynamic<T1, T2, T3, T4, T5, TResult>(this object container, string methodName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return
                (TResult)
                    Delegate.CreateDelegate(typeof (Func<T1, T2, T3, T4, T5, TResult>), container, methodName)
                        .DynamicInvoke(arg1, arg2, arg3, arg4, arg5);
        }

        public static void Dynamic<T>(this object container, string methodName, T arg)
        {
            Delegate.CreateDelegate(typeof(Action<T>), container, methodName).DynamicInvoke(arg);
        }

        public static void Dynamic<T1, T2>(this object container, string methodName, T1 arg1, T2 arg2)
        {
            Delegate.CreateDelegate(typeof(Action<T1, T2>), container, methodName).DynamicInvoke(arg1, arg2);
        }
        public static void Dynamic<T1, T2, T3>(this object container, string methodName, T1 arg1, T2 arg2, T3 arg3)
        {
            Delegate.CreateDelegate(typeof(Action<T1, T2, T3>), container, methodName).DynamicInvoke(arg1, arg2, arg3);
        }
        public static void Dynamic<T1, T2, T3, T4>(this object container, string methodName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4>), container, methodName).DynamicInvoke(arg1, arg2, arg3, arg4);
        }
        public static void Dynamic<T1, T2, T3, T4, T5>(this object container, string methodName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4, T5>), container, methodName).DynamicInvoke(arg1, arg2, arg3, arg4, arg5);
        }
    }
}
