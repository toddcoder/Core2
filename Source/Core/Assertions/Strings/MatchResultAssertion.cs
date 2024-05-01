﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Matching;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Assertions.Strings;

public class MatchResultAssertion : IAssertion<MatchResult>
{
   protected MatchResult result;
   protected List<Constraint> constraints;
   protected bool not;
   protected string name;

   public MatchResultAssertion(MatchResult result)
   {
      this.result = result;

      constraints = [];
      not = false;
      name = "Result";
   }

   public MatchResultAssertion Not
   {
      get
      {
         not = true;
         return this;
      }
   }

   public bool BeEquivalentToTrue() => beEquivalentToTrue(this);

   public MatchResult Value => result;

   public IEnumerable<Constraint> Constraints => constraints;

   public IAssertion<MatchResult> Named(string name)
   {
      this.name = name;
      return this;
   }

   protected MatchResultAssertion add(Func<bool> constraintFunction, string message)
   {
      constraints.Add(new Constraint(constraintFunction, message, not, name, Value));
      not = false;

      return this;
   }

   public MatchResultAssertion HaveMatchCountOf(int matchCount)
   {
      return add(() => result.MatchCount >= matchCount, $"$name must $not have a match count of at least {matchCount}");
   }

   public MatchResultAssertion HaveGroupCountOf(int groupCount)
   {
      return HaveMatchCountOf(1)
         .add(() => result.GroupCount(0) >= groupCount, $"$name must $not have a group count of at least {groupCount}");
   }

   public void OrThrow() => orThrow(this);

   public void OrThrow(string message) => orThrow(this, message);

   public void OrThrow(Func<string> messageFunc) => orThrow(this, messageFunc);

   public void OrThrow<TException>(params object[] args) where TException : Exception => orThrow<TException, MatchResult>(this, args);

   public MatchResult Force() => force(this);

   public MatchResult Force(string message) => force(this, message);

   public MatchResult Force(Func<string> messageFunc) => force(this, messageFunc);

   public MatchResult Force<TException>(params object[] args) where TException : Exception => force<TException, MatchResult>(this, args);

   public TResult Force<TResult>() => throw fail("Can't convert Result to another type");

   public TResult Force<TResult>(string message) => throw fail(message);

   public TResult Force<TResult>(Func<string> messageFunc) => throw fail(messageFunc());

   public TResult Force<TException, TResult>(params object[] args) where TException : Exception => Force<TResult>();

   public Result<MatchResult> OrFailure() => orFailure(this);

   public Result<MatchResult> OrFailure(string message) => orFailure(this, message);

   public Result<MatchResult> OrFailure(Func<string> messageFunc) => orFailure(this, messageFunc);

   public Maybe<MatchResult> OrNone() => orNone(this);

   public Optional<MatchResult> OrEmpty() => orEmpty(this);

   public Optional<MatchResult> OrFailed() => orFailed(this);

   public Optional<MatchResult> OrFailed(string message) => orFailed(this, message);

   public Optional<MatchResult> OrFailed(Func<string> messageFunc) => orFailed(this, messageFunc);

   public async Task<Completion<MatchResult>> OrFailureAsync(CancellationToken token) => await orFailureAsync(this, token);

   public async Task<Completion<MatchResult>> OrFailureAsync(string message, CancellationToken token) =>
      await orFailureAsync(this, message, token);

   public async Task<Completion<MatchResult>> OrFailureAsync(Func<string> messageFunc, CancellationToken token)
   {
      return await orFailureAsync(this, messageFunc, token);
   }

   public bool OrReturn() => orReturn(this);
}