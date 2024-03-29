using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class Main_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::Main);
            args = new Type[]{};
            method = type.GetMethod("EnterGame", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, EnterGame_0);

            field = type.GetField("GameVersion", flag);
            app.RegisterCLRFieldGetter(field, get_GameVersion_0);
            app.RegisterCLRFieldSetter(field, set_GameVersion_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_GameVersion_0, AssignFromStack_GameVersion_0);


        }


        static StackObject* EnterGame_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::Main instance_of_this_method = (global::Main)typeof(global::Main).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.EnterGame();

            return __ret;
        }


        static object get_GameVersion_0(ref object o)
        {
            return ((global::Main)o).GameVersion;
        }

        static StackObject* CopyToStack_GameVersion_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::Main)o).GameVersion;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_GameVersion_0(ref object o, object v)
        {
            ((global::Main)o).GameVersion = (System.String)v;
        }

        static StackObject* AssignFromStack_GameVersion_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @GameVersion = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((global::Main)o).GameVersion = @GameVersion;
            return ptr_of_this_method;
        }



    }
}
