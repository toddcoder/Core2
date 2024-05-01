using System.Reflection;
using Core.Assertions;
using Core.Collections;
using Core.Computers;
using Core.Configurations;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Monads.Lazy;
using static System.Reflection.Assembly;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Services;

public class TypeManager
{
   protected static StringHash assemblyNamesFrom(Setting assembliesSetting)
   {
      return assembliesSetting.Settings().ToStringHash(i => i.key, i => i.setting.Value.String("path"));
   }

   protected static StringHash typeNamesFrom(Setting typesSetting)
   {
      return typesSetting.Items().ToStringHash(i => i.key, i => i.text);
   }

   public static Result<TypeManager> FromConfiguration(Configuration configuration)
   {
      return
         from assembliesSetting in configuration.Result.Setting("assemblies")
         let assemblyNames = assemblyNamesFrom(assembliesSetting)
         from assertedAssemblies in assemblyNames.Must().HaveCountOf(1).OrFailure()
         from typesSetting in configuration.Result.Setting("types")
         let typeNames = typeNamesFrom(typesSetting)
         from assertedTypes in typeNames.Must().HaveCountOf(1).OrFailure()
         select new TypeManager(assemblyNames, typeNames);
   }

   protected StringHash<Assembly> assemblyCache;
   protected StringHash assemblyNames;
   protected StringHash<Type> typeCache;
   protected StringHash typeNames;
   protected Maybe<string> _defaultAssemblyName;
   protected Maybe<string> _defaultTypeName;

   public TypeManager(StringHash assemblyNames, StringHash typeNames)
   {
      this.assemblyNames = assemblyNames;
      this.typeNames = typeNames;

      assemblyCache = [];
      _defaultAssemblyName = assemblyNames.Tuples().FirstOrNone(i => i.key == "default").Map((_, path) => path);

      typeCache = [];
      _defaultTypeName = typeNames.Tuples().FirstOrNone(i => i.key == "default").Map((_, typeName) => typeName);
   }

   public Result<Type> Type(string assemblyName, string typeName)
   {
      if (assemblyName == "$")
      {
         return getTypeName(typeName).Map(n => System.Type.GetType(n)!);
      }
      else
      {
         return
            from assembly in getAssemblyFromCache(assemblyName)
            from type in getTypeFromCache(typeName, assembly)
            select type;
      }
   }

   protected Result<Type> getTypeFromCache(string name, Assembly assembly)
   {
      try
      {
         LazyMaybe<Type> _type = nil;
         LazyResult<string> _typeName = nil;
         if (_type.ValueOf(typeCache.Maybe[name]) is (true, var type))
         {
            return type;
         }
         else if (_typeName.ValueOf(getTypeName(name)) is (true, var typeName))
         {
            LazyMaybe<MatchResult> _result = nil;
            LazyResult<Type> _assemblyType = nil;

            if (_result.ValueOf(typeName.Matches("^ -/{<} '<' -/{:} ':' /s* -/{>} '>' $; f")) is
                (true, var (possibleTypeName, subTypeName, subAssemblyName)))
            {
               typeName = $"{possibleTypeName}`1";

               var _genericType =
                  from typeFromAssembly in getTypeFromAssembly(assembly, typeName)
                  from subType in Type(subAssemblyName, subTypeName)
                  select typeFromAssembly.MakeGenericType(subType);
               if (_genericType is (true, var genericType))
               {
                  typeCache[name] = genericType;
               }

               return _genericType;
            }
            else if (_assemblyType.ValueOf(getTypeFromAssembly(assembly, typeName)))
            {
               typeCache[name] = _assemblyType;
            }

            return _assemblyType;
         }
         else
         {
            return _typeName.Exception;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   protected Result<string> getTypeName(string name) => typeNames.Items[name].Result($"Couldn't determine type name {name}");

   protected static Result<Type> getTypeFromAssembly(Assembly assembly, string typeName) => tryTo(() => assembly.GetType(typeName, true)!);

   protected Result<Assembly> getAssemblyFromCache(string name)
   {
      try
      {
         LazyMaybe<Assembly> _assembly = nil;
         LazyMaybe<string> _path = nil;
         if (_assembly.ValueOf(assemblyCache.Maybe[name]) is (true, var assembly))
         {
            return assembly;
         }
         else if (_path.ValueOf(assemblyNames.Maybe[name]) is (true, var path))
         {
            FolderName.Current = ((FileName)path).Folder;
            assembly = LoadFrom(path);
            assemblyCache[name] = assembly;

            return assembly;
         }
         else
         {
            return fail($"Couldn't find assembly named {name}");
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Maybe<string> DefaultAssemblyName => _defaultAssemblyName;

   public Maybe<string> DefaultTypeName => _defaultTypeName;
}