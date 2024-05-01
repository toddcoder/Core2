﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Assertions;
using Core.Collections;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static System.Reflection.BindingFlags;
using static System.Reflection.MemberTypes;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Objects;

public class ReflectorFormat
{
   protected class Pair
   {
      public Pair(ReflectorReplacement replacement, IGetter getter)
      {
         Replacement = replacement;
         Getter = getter;
      }

      public ReflectorReplacement Replacement { get; }

      public IGetter Getter { get; }

      public void Replace(object obj, Slicer slicer) => Replacement.Replace(obj, Getter, slicer);
   }

   protected class MemberData
   {
      public MemberData(Hash<string, Pair> pairs, string source)
      {
         Pairs = pairs;
         Source = source;
      }

      public Hash<string, Pair> Pairs { get; }

      public string Source { get; }
   }

   protected class Replacements
   {
      public Replacements(IEnumerable<ReflectorReplacement> replacements, string source)
      {
         ReflectorReplacements = [.. replacements];
         Source = source;
      }

      public ReflectorReplacement[] ReflectorReplacements { get; }

      public string Source { get; }
   }

   public static Result<ReflectorFormat> GetReflector(object obj) =>
      from nonNullObject in obj.Must().Not.BeNull().OrFailure()
      from type in tryTo(nonNullObject.GetType)
      select new ReflectorFormat(nonNullObject, type);

   protected static Result<Replacements> getReplacements(string source)
   {
      var _matches = source.Matches(@"-(< '\') '{' /(-['}']+) '}'; f").Map(r => r.Matches);
      if (_matches)
      {
         var replacements = getReplacements(_matches);
         return new Replacements(replacements, source);
      }
      else
      {
         return fail("Couldn't find any replacements");
      }
   }

   protected static IEnumerable<ReflectorReplacement> getReplacements(Match[] matches)
   {
      return matches.Select(match => new ReflectorReplacement(match.Index, match.Length, match.Groups[1]));
   }

   protected static Result<MemberData> getMembers(Type type, string template)
   {
      Hash<string, Pair> members = [];
      const MemberTypes memberTypes = Field | Property;
      const BindingFlags bindingFlags = BindingFlags.Instance | GetField | GetProperty | NonPublic | Public;

      var _replacements = getReplacements(template);
      if (_replacements is (true, var replacements))
      {
         foreach (var reflectorReplacement in replacements.ReflectorReplacements)
         {
            var memberInfos = type.GetMember(reflectorReplacement.MemberName, memberTypes, bindingFlags);
            if (memberInfos.Length != 0)
            {
               Maybe<IGetter> _chosen = nil;
               foreach (var info in memberInfos)
               {
                  if (info is FieldInfo fieldInfo)
                  {
                     _chosen = new FieldGetter(fieldInfo).Some<IGetter>();
                     break;
                  }

                  if (info is PropertyInfo propertyInfo)
                  {
                     _chosen = new PropertyGetter(propertyInfo).Some<IGetter>();
                     break;
                  }
               }

               if (_chosen is (true, var chosen))
               {
                  members[reflectorReplacement.MemberName] = new Pair(reflectorReplacement, chosen);
               }
               else
               {
                  return failedFind(type, reflectorReplacement.MemberName);
               }
            }
            else
            {
               return failedFind(type, reflectorReplacement.MemberName);
            }
         }

         return new MemberData(members, replacements.Source);
      }
      else
      {
         return _replacements.Exception;
      }
   }

   protected static Result<MemberData> failedFind(Type type, string memberName) => fail($"Member {memberName} in type {type} couldn't be found");

   protected object obj;
   protected Type type;

   protected ReflectorFormat(object obj, Type type)
   {
      this.obj = obj;
      this.type = type;
   }

   public Result<string> Format(string template) =>
      tryTo(() => from memberData in getMembers(type, template)
         from formatted in getText(memberData)
         select formatted.Substitute(@"'\{'; f", "{"));

   protected Result<string> getText(MemberData memberData) => tryTo(() =>
   {
      var slicer = new Slicer(memberData.Source);

      foreach (var item in memberData.Pairs)
      {
         item.Value.Replace(obj, slicer);
      }

      return slicer.ToString();
   });

   protected Result<object> getValue(MemberInfo info) => info switch
   {
      FieldInfo fieldInfo => fieldInfo.GetValue(obj)!,
      PropertyInfo propertyInfo => propertyInfo.GetValue(obj)!,
      _ => fail($"Couldn't invoke member {info.Name}")
   };
}