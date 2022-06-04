using System.Reflection;
using Reck.Attributes;

namespace Reck.Enums;

public class HttpUrlParameter
{
    public string Name { get; private set; }
    public Type Type { get; private set; }
    public object DefaultValue { get; private set; }
    public bool IsOptional { get; private set; } = false;

    public HttpUrlParameter(string name, Type type, bool isOptional, object defaultValue)
    {
        Name = name.ToLower();
        Type = type;
        IsOptional = isOptional;
        DefaultValue = defaultValue;
    }

    internal HttpUrlParameter(ParameterInfo parameterInfo)
    {
        Name = parameterInfo.Name.ToLower();
        Type = parameterInfo.ParameterType;
        IsOptional = !(parameterInfo.GetCustomAttribute(typeof(OptionalParam)) is null);

        //  The default value is the value given to the ApiEndPoint if no value is specified.
        //  It prioritizes user-defined default value ( ex: public ...(int param = -1) )
        //  If no value type is specified, we choose one based on whether the parameter's type is value or reference
        //  We get the default value type's value with Activator.CreateInstance, reference default value has to be null
        DefaultValue = (parameterInfo.HasDefaultValue) ? parameterInfo.DefaultValue :
            (parameterInfo.ParameterType.IsValueType) ? Activator.CreateInstance(parameterInfo.ParameterType) : null;
        //              User-defined default value                                                                                  Default value type with Activator                     null
    }
}
