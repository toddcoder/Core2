using Core.Applications.Messaging;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Forms;

public class LazySingletonForm<T> where T : Form
{
   protected Maybe<T> _reference = nil;

   public readonly MessageEvent Created = new();
   public readonly MessageEvent AfterReset = new();
   public readonly MessageEvent AfterShow = new();
   public readonly MessageEvent AfterFocus = new();

   public void Show(Func<T> initializer)
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
         AfterFocus.Invoke();
      }
      else if (!reference.IsDisposed)
      {
         reference.Show();
         AfterShow.Invoke();
      }
      else
      {
         reference = initializer();
         _reference = reference;
         reference.Show();
         AfterShow.Invoke();
      }
   }

   public void Reset()
   {
      _reference = nil;
      AfterReset.Invoke();
   }

   public bool Available => _reference is (true, { IsDisposed: false, Visible: true });
}