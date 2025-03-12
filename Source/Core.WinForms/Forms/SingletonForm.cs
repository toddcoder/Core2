using Core.Applications.Messaging;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Forms;

public class SingletonForm<T>(Func<T> initializer) where T : Form
{
   protected Func<T> initializer = initializer;
   protected Maybe<T> _reference = nil;

   public Maybe<T> Reference => _reference;

   public readonly MessageEvent Created = new();
   public readonly MessageEvent WasReset = new();

   public void Show()
   {
      var uninitialized = !_reference;
      (var reference, _reference) = _reference.Create(initializer);
      if (uninitialized)
      {
         reference.FormClosing += (_, _) => Reset();
         Created.Invoke();
      }

      if (reference.Visible)
      {
         reference.Focus();
      }
      else if (!reference.IsDisposed)
      {
         reference.Show();
      }
      else
      {
         reference = initializer();
         _reference = reference;
         reference.Show();
      }
   }

   public void Reset()
   {
      _reference = nil;
      WasReset.Invoke();
   }

   public bool Available => _reference is (true, { IsDisposed: false, Visible: true });
}