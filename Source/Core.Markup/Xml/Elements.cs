﻿using System.Text;
using Core.Assertions;
using Core.Computers;
using Core.Enumerables;

namespace Core.Markup.Xml;

public class Elements : IRendering
{
   public class ElementEventArgs : EventArgs
   {
      protected Element element;

      public ElementEventArgs(Element element) => this.element = element;

      public Element Element => element;
   }

   protected List<Element> elements;
   protected bool isHtml;

   public event EventHandler<ElementEventArgs>? ElementAdded;

   public Elements()
   {
      elements = [];
      isHtml = false;
   }

   public Element this[string name]
   {
      get
      {
         var _value = elements.Where(e => e.Name == name).FirstOrNone();
         if (_value is (true, var element))
         {
            return element;
         }
         else
         {
            element = new Element { Name = name, Text = "" };
            elements.Add(element);

            return element;
         }
      }
   }

   public Element this[int index] => elements[index];

   public Element Add(Element element)
   {
      element.Must().Not.BeNull().OrThrow();

      elements.Add(element);
      ElementAdded?.Invoke(this, new ElementEventArgs(element));

      return element;
   }

   public Element Add(string name) => Add(name, string.Empty);

   public Element Add(string name, MarkupTextHolder text)
   {
      var element = new Element { Name = name, Text = text };
      elements.Add(element);
      ElementAdded?.Invoke(this, new ElementEventArgs(element));

      return element;
   }

   public bool IsHtml
   {
      get => isHtml;
      set
      {
         isHtml = value;
         foreach (var element in elements)
         {
            element.Siblings.IsHtml = value;
            element.Children.IsHtml = value;
         }
      }
   }

   public IEnumerable<Element> All => elements.Select(e => e);

   public bool IsEmpty => elements.Count == 0;

   public bool HasContent => elements.Any();

   public void Clear() => elements.Clear();

   public override string ToString() => ToStringRendering(_ => true);

   public string ToStringRendering(Func<Element, bool> callback)
   {
      var builder = new StringBuilder();
      foreach (var element in All)
      {
         builder.Append(element.ToStringRendering(callback));
      }

      return builder.ToString();
   }

   public void RenderToFile(FileName file) => RenderToFile(file, _ => true);

   public void RenderToFile(FileName file, Func<Element, bool> callback)
   {
      foreach (var element in All)
      {
         element.RenderToFile(file, callback);
      }
   }
}