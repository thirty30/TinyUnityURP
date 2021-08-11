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
    unsafe class TFramework_TNet_TNetMessageHead_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(TFramework.TNet.TNetMessageHead);
            args = new Type[]{typeof(System.Byte[]), typeof(System.Int32)};
            method = type.GetMethod("Deserialize", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Deserialize_0);
            args = new Type[]{};
            method = type.GetMethod("GetHeadSize", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetHeadSize_1);
            args = new Type[]{typeof(System.Byte[]), typeof(System.Int32)};
            method = type.GetMethod("Serialize", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Serialize_2);

            field = type.GetField("MsgID", flag);
            app.RegisterCLRFieldGetter(field, get_MsgID_0);
            app.RegisterCLRFieldSetter(field, set_MsgID_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_MsgID_0, AssignFromStack_MsgID_0);
            field = type.GetField("BodySize", flag);
            app.RegisterCLRFieldGetter(field, get_BodySize_1);
            app.RegisterCLRFieldSetter(field, set_BodySize_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_BodySize_1, AssignFromStack_BodySize_1);
            field = type.GetField("IsCompressed", flag);
            app.RegisterCLRFieldGetter(field, get_IsCompressed_2);
            app.RegisterCLRFieldSetter(field, set_IsCompressed_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_IsCompressed_2, AssignFromStack_IsCompressed_2);

            args = new Type[]{};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }


        static StackObject* Deserialize_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @aIndex = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Byte[] @aBuffer = (System.Byte[])typeof(System.Byte[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            TFramework.TNet.TNetMessageHead instance_of_this_method = (TFramework.TNet.TNetMessageHead)typeof(TFramework.TNet.TNetMessageHead).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Deserialize(@aBuffer, @aIndex);

            return __ret;
        }

        static StackObject* GetHeadSize_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            TFramework.TNet.TNetMessageHead instance_of_this_method = (TFramework.TNet.TNetMessageHead)typeof(TFramework.TNet.TNetMessageHead).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetHeadSize();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* Serialize_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @aIndex = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Byte[] @aBuffer = (System.Byte[])typeof(System.Byte[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            TFramework.TNet.TNetMessageHead instance_of_this_method = (TFramework.TNet.TNetMessageHead)typeof(TFramework.TNet.TNetMessageHead).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Serialize(@aBuffer, @aIndex);

            return __ret;
        }


        static object get_MsgID_0(ref object o)
        {
            return ((TFramework.TNet.TNetMessageHead)o).MsgID;
        }

        static StackObject* CopyToStack_MsgID_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((TFramework.TNet.TNetMessageHead)o).MsgID;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_MsgID_0(ref object o, object v)
        {
            ((TFramework.TNet.TNetMessageHead)o).MsgID = (System.Int32)v;
        }

        static StackObject* AssignFromStack_MsgID_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @MsgID = ptr_of_this_method->Value;
            ((TFramework.TNet.TNetMessageHead)o).MsgID = @MsgID;
            return ptr_of_this_method;
        }

        static object get_BodySize_1(ref object o)
        {
            return ((TFramework.TNet.TNetMessageHead)o).BodySize;
        }

        static StackObject* CopyToStack_BodySize_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((TFramework.TNet.TNetMessageHead)o).BodySize;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_BodySize_1(ref object o, object v)
        {
            ((TFramework.TNet.TNetMessageHead)o).BodySize = (System.Int32)v;
        }

        static StackObject* AssignFromStack_BodySize_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @BodySize = ptr_of_this_method->Value;
            ((TFramework.TNet.TNetMessageHead)o).BodySize = @BodySize;
            return ptr_of_this_method;
        }

        static object get_IsCompressed_2(ref object o)
        {
            return ((TFramework.TNet.TNetMessageHead)o).IsCompressed;
        }

        static StackObject* CopyToStack_IsCompressed_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((TFramework.TNet.TNetMessageHead)o).IsCompressed;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_IsCompressed_2(ref object o, object v)
        {
            ((TFramework.TNet.TNetMessageHead)o).IsCompressed = (System.Byte)v;
        }

        static StackObject* AssignFromStack_IsCompressed_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Byte @IsCompressed = (byte)ptr_of_this_method->Value;
            ((TFramework.TNet.TNetMessageHead)o).IsCompressed = @IsCompressed;
            return ptr_of_this_method;
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);

            var result_of_this_method = new TFramework.TNet.TNetMessageHead();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
