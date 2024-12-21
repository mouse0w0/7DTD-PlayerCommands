using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace PlayerCommands;

public class MyContractResolver : DefaultContractResolver
{
    protected override List<MemberInfo> GetSerializableMembers(Type objectType)
    {
        var members = base.GetSerializableMembers(objectType);
        members.RemoveAll(memberInfo =>
        {
            switch (memberInfo)
            {
                case PropertyInfo propertyInfo:
                    if (!propertyInfo.CanWrite) return true;
                    break;
            }

            return false;
        });
        return members;
    }
}