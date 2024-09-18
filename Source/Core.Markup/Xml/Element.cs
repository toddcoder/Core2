﻿using System.Text;
using Core.Computers;
using Core.Matching;
using Core.Monads;
using Core.Monads.Lazy;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Xml;

public class Element : IRendering
{
   public static implicit operator Element(string source)
   {
      var _tag = source.Matches("^ /(['a-zA-Z_'] [/w '-']*) /s* '>' /s* [quote]? /(.*) $; f");
      LazyMaybe<MatchResult> _link = nil;

      if (_tag is (true, var (name, text)))
      {
         return new Element
         {
            Name = name,
            Text = text
         };
      }
      else if (_link.ValueOf(source.Matches("^ 'link|' /(-['|']+) ('|' /(.+))? $; f")) is (true, var (link, linkText)))
      {
         return new Element
         {
            Name = "a",
            Text = linkText
         } + $"@href={link}";
      }
      else
      {
         throw new ApplicationException($"Didn't understand {source}");
      }
   }

   public static Element operator +(Element element, string source)
   {
      if (Xml.Attribute.Matches(source))
      {
         element.Attribute = source;
         return element;
      }
      else
      {
         Element childElement = source;
         element.Children.Add(childElement);

         return childElement;
      }
   }

   public static Element operator *(Element element, string source)
   {
      if (Xml.Attribute.Matches(source))
      {
         element.Attribute = source;
         return element;
      }
      else
      {
         Element childElement = source;
         element.Children.Add(childElement);

         return element;
      }
   }

   protected string name = "no-name";
   protected MarkupTextHolder text = "";
   protected Maybe<Element> _parent = nil;
   protected Elements siblings = new();
   protected Elements children = new();
   protected Attributes attributes = [];

   public Element()
   {
      siblings.ElementAdded += (_, e) =>
      {
         if (_parent is (true, var parent))
         {
            e.Element.Parent = parent;
         }
      };
      children.ElementAdded += (_, e) => e.Element.Parent = this;
   }

   public Element this[string elementName] => Children[elementName];

   public Element this[int index] => Children[index];

   public string Child
   {
      set
      {
         Element element = value;
         children.Add(element);
      }
   }

   public string Name
   {
      get => name;
      set => name = value;
   }

   public MarkupTextHolder Text
   {
      get => text;
      set => text = value;
   }

   public Maybe<Element> Parent
   {
      get => _parent;
      set => _parent = value;
   }

   public Elements Siblings => siblings;

   public Elements Children => children;

   public Attributes Attributes => attributes;

   public string Attribute
   {
      set
      {
         Attribute attribute = value;
         attributes[attribute.Name] = attribute;
      }
   }

   public override string ToString() => ToStringRendering(_ => true);

   public virtual string ToStringRendering(Func<Element, bool> callback)
   {
      if (callback(this))
      {
         var element = new StringBuilder("<");

         element.Append(name);

         element.Append(attributes);

         var closed = children.IsEmpty && text.Text.IsEmpty();
         element.Append(closed ? " />" : ">");

         if (text.Text.IsNotEmpty())
         {
            element.Append(text.Text);
         }

         element.Append(children.ToStringRendering(callback));

         if (!closed)
         {
            element.Append($"</{name}>");
         }

         element.Append(siblings.ToStringRendering(callback));

         return element.ToString();
      }
      else
      {
         return "";
      }
   }

   public virtual void RenderToFile(FileName file) => RenderToFile(file, _ => true);

   public virtual void RenderToFile(FileName file, Func<Element, bool> callback)
   {
      if (callback(this))
      {
         file.Append("<");
         file.Append(name);

         file.Append(attributes.ToString());

         var closed = children.IsEmpty && text.Text.IsEmpty();
         file.Append(closed ? " />" : ">");

         if (text.Text.IsNotEmpty())
         {
            file.Append(text.Text);
         }

         children.RenderToFile(file, callback);

         if (!closed)
         {
            file.Append($"</{name}>");
         }

         siblings.RenderToFile(file, callback);
      }
   }
}