using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Forms;

public class SingletonForm<T>(Func<T> initializer) where T : Form
{
   protected Func<T> initializer = initializer;
   protected Maybe<T> _reference = nil;

   public Maybe<T> Reference => _reference;

   public void Show()
   {
      (var reference, _reference) = _reference.Create(initializer);

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

   public void Reset() => _reference = nil;
}