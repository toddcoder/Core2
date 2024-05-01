using Core.Configurations;
using Core.Matching;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Parameters;

public class Parameter : PropertyInterface
{
   public static Maybe<Parameter> FromString(string input)
   {
      var _result = input.Matches("^ '@'? /(/w+) /s* ('[' /(/w+) ']')? /s* ':' /s* /('$'? [/w '.']+) ('(' /(/d+) ')')? (/s+ /('output'))? $; f");
      if (_result is (true, var result))
      {
         var name = result.FirstGroup;
         var signature = result.SecondGroup;
         if (signature.IsEmpty())
         {
            signature = name.ToTitleCase();
         }

         var typeName = fixTypeName(result.ThirdGroup);
         var _type = getType(typeName);
         var _size = result.FourthGroup.Maybe().Int32();
         var output = result.FifthGroup.Same("output");

         return new Parameter(name, signature)
         {
            Type = _type,
            Size = _size,
            Output = output,
            Value = nil,
            Default = nil
         };
      }
      else
      {
         return nil;
      }
   }

   public static Parameter Parse(Setting parameterSetting)
   {
      var name = parameterSetting.Key;
      var signature = parameterSetting.Maybe.String("signature") | name;
      var typeName = parameterSetting.Maybe.String("type") | "$string";
      typeName = fixTypeName(typeName);
      var _type = getType(typeName);
      var _size = parameterSetting.Maybe.Int32("size");
      var output = parameterSetting.Value.Boolean("output");
      var _value = parameterSetting.Maybe.String("value");
      var _default = parameterSetting.Maybe.String("default");

      return new Parameter(name, signature)
      {
         Type = _type,
         Size = _size,
         Output = output,
         Value = _value,
         Default = _default
      };
   }

   protected static Maybe<Type> getType(string typeName)
   {
      return maybe<Type>() & typeName.IsNotEmpty() & (() => System.Type.GetType(typeName, true, true)!);
   }

   protected static string fixTypeName(string typeName)
   {
      typeName = typeName.Substitute("^ '$'; f", "System.");
      return typeName.IsNotEmpty() && !typeName.Has(".") ? "System." + typeName : typeName;
   }

   public Parameter(string name, string signature) : base(name, signature)
   {
      Type = nil;
      Size = nil;
      Output = false;
      Value = nil;
      Default = nil;
   }

   public Parameter(string name, string signature, Type type) : base(name, signature)
   {
      Type = type;
      Size = nil;
      Output = false;
      Value = nil;
      Default = nil;
   }

   public Maybe<Type> Type { get; set; }

   public Maybe<int> Size { get; init; }

   public bool Output { get; init; }

   public Maybe<string> Value { get; init; }

   public Maybe<string> Default { get; init; }
}