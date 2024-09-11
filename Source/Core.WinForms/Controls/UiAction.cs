using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using Core.Arrays;
using Core.Collections;
using Core.Computers;
using Core.DataStructures;
using Core.Dates.DateIncrements;
using Core.Enumerables;
using Core.Monads;
using Core.Monads.Lazy;
using Core.Numbers;
using Core.Strings;
using Core.Strings.Emojis;
using Core.WinForms.ControlWrappers;
using Core.WinForms.Drawing;
using static Core.Lambdas.LambdaFunctions;
using static Core.Monads.MonadFunctions;
using SolidBrush = System.Drawing.SolidBrush;
using Timer = System.Windows.Forms.Timer;

namespace Core.WinForms.Controls;

public class UiAction : UserControl, ISubTextHost, IButtonControl, IHasObjectId
{
   protected const float START_AMOUNT = .9f;

   protected static Hash<UiActionType, Color> globalForeColors = new()
   {
      [UiActionType.Uninitialized] = Color.White,
      [UiActionType.Message] = Color.White,
      [UiActionType.Exception] = Color.White,
      [UiActionType.Success] = Color.White,
      [UiActionType.Failure] = Color.Black,
      [UiActionType.NoStatus] = Color.Black,
      [UiActionType.Selected] = Color.White,
      [UiActionType.Unselected] = Color.White,
      [UiActionType.ProgressIndefinite] = Color.White,
      [UiActionType.ProgressDefinite] = Color.White,
      [UiActionType.BusyText] = Color.White,
      [UiActionType.Automatic] = Color.Black,
      [UiActionType.Disabled] = Color.LightGray,
      [UiActionType.Caution] = Color.White,
      [UiActionType.ControlLabel] = Color.White,
      [UiActionType.Button] = Color.Black,
      [UiActionType.Console] = Color.White,
      [UiActionType.Busy] = Color.White,
      [UiActionType.MuteProgress] = Color.White,
      [UiActionType.Divider] = Color.Black
   };
   protected static Hash<UiActionType, Color> globalBackColors = new()
   {
      [UiActionType.Uninitialized] = Color.Gray,
      [UiActionType.Message] = Color.Blue,
      [UiActionType.Exception] = Color.Red,
      [UiActionType.Success] = Color.Green,
      [UiActionType.Failure] = Color.Gold,
      [UiActionType.NoStatus] = Color.White,
      [UiActionType.Selected] = Color.FromArgb(0, 127, 0),
      [UiActionType.Unselected] = Color.FromArgb(127, 0, 0),
      [UiActionType.Automatic] = Color.White,
      [UiActionType.Disabled] = Color.DarkGray,
      [UiActionType.Caution] = Color.CadetBlue,
      [UiActionType.ControlLabel] = Color.CadetBlue,
      [UiActionType.Button] = Color.LightGray,
      [UiActionType.Console] = Color.Blue,
      [UiActionType.Busy] = Color.Teal,
      [UiActionType.BusyText] = Color.Teal,
      [UiActionType.ProgressDefinite] = Color.CadetBlue,
      [UiActionType.ProgressIndefinite] = Color.CadetBlue,
      [UiActionType.MuteProgress] = Color.CadetBlue,
      [UiActionType.Divider] = Color.White
   };
   protected static Hash<UiActionType, MessageStyle> globalStyles = new()
   {
      [UiActionType.Uninitialized] = MessageStyle.Italic,
      [UiActionType.Message] = MessageStyle.None,
      [UiActionType.Exception] = MessageStyle.Bold,
      [UiActionType.Success] = MessageStyle.Bold,
      [UiActionType.Failure] = MessageStyle.Bold,
      [UiActionType.NoStatus] = MessageStyle.Bold,
      [UiActionType.BusyText] = MessageStyle.ItalicBold,
      [UiActionType.Caution] = MessageStyle.Bold,
      [UiActionType.ControlLabel] = MessageStyle.Bold,
      [UiActionType.Http] = MessageStyle.Bold
   };
   protected static BusyStyle busyStyle = BusyStyle.Default;

   public static Hash<UiActionType, Color> GlobalForeColors => globalForeColors;

   public static Hash<UiActionType, Color> GlobalBackColors => globalBackColors;

   public static Hash<UiActionType, MessageStyle> GlobalStyles => globalStyles;

   public static Maybe<Form> MainForm { get; set; } = nil;

   public static BusyStyle BusyStyle
   {
      get => busyStyle;
      set => busyStyle = value;
   }

   protected Font font = new("Consolas", 12);
   protected Font italicFont = new("Consolas", 12, FontStyle.Italic);
   protected Font boldFont = new("Consolas", 12, FontStyle.Bold);
   protected Font italicBoldFont = new("Consolas", 12, FontStyle.Italic | FontStyle.Bold);
   protected AutoHash<UiActionType, Color> foreColors = new(mlt => globalForeColors[mlt]);
   protected AutoHash<UiActionType, Color> backColors = new(mlt => globalBackColors[mlt]);
   protected AutoHash<UiActionType, MessageStyle> styles = new(mlt => globalStyles[mlt]);
   protected string text = string.Empty;
   protected UiActionType type = UiActionType.Uninitialized;
   protected int value;
   protected Timer timerPaint = new()
   {
      Interval = 100,
      Enabled = false
   };
   protected Timer timer = new()
   {
      Interval = 1000,
      Enabled = false
   };
   protected int maximum = 1;
   protected int index = 1;
   protected bool mouseInside;
   protected bool mouseDown;
   protected UiToolTip toolTip;
   internal Maybe<string> _clickText = nil;
   protected LazyMaybe<BusyTextProcessor> _busyTextProcessor = nil;
   protected LazyMaybe<ProgressDefiniteProcessor> _progressDefiniteProcessor = nil;
   protected LazyMaybe<ProgressMiniProcessor> _progressMiniProcessor = nil;
   protected LazyMaybe<BusyProcessor> _busyProcessor = nil;
   protected Maybe<int> _percentage = nil;
   protected Maybe<Color> _foreColor = nil;
   protected Maybe<Color> _backColor = nil;
   protected Maybe<MessageStyle> _style = nil;
   protected Maybe<UiActionType> _lastType = nil;
   protected Maybe<bool> _lastEnabled = nil;
   protected Maybe<Color> _lastForeColor = nil;
   protected Maybe<Color> _lastBackColor = nil;
   protected Maybe<MessageStyle> _lastStyle = nil;
   protected Maybe<Image> _image = nil;
   protected Hash<Guid, SubText> subTexts = [];
   protected Lazy<Stopwatch> stopwatch = new(() => new Stopwatch());
   protected Lazy<BackgroundWorker> backgroundWorker;
   protected bool oneTimeTimer;
   protected Maybe<string> _workingText = nil;
   protected Maybe<SubText> _working = nil;
   protected int workingAlpha = 255;
   protected Timer workingTimer = new() { Interval = 2000 };
   protected CardinalAlignment workingAlignment = CardinalAlignment.SouthWest;
   protected MaybeStack<SubText> legends = [];
   protected bool isDirty;
   protected CheckStyle checkStyle = CheckStyle.None;
   protected Guid id = Guid.NewGuid();
   protected bool httpHandlerAdded;
   protected bool isUrlGood;
   protected Lazy<Font> marqueeFont = new(() => new Font("Consolas", 8));
   protected Lazy<UiActionScroller> scroller;
   protected Maybe<string> _successToolTip = nil;
   protected Maybe<string> _failureToolTip = nil;
   protected Maybe<string> _exceptionToolTip = nil;
   protected Maybe<string> _noStatusToolTip = nil;
   protected Maybe<SubText> _failureSubText = nil;
   protected Maybe<SubText> _exceptionSubText = nil;
   protected Maybe<SubText> _noStatusSubText = nil;
   protected Maybe<string> _oldTitle = nil;
   protected Maybe<SubText> _progressSubText = nil;
   protected bool flipOn;
   protected Maybe<SubText> _flipFlop = nil;
   protected bool clickToCancel;
   protected Optional<TaskBarProgress> _taskBarProgress = nil;
   protected bool cancelled = true;
   protected Rectangle[] rectangles = [];
   protected Maybe<int> _floor = nil;
   protected Maybe<int> _ceiling = nil;
   protected Maybe<KeyMatch> _keyMatch = nil;
   protected Maybe<SymbolWriter> _symbolWriter = nil;
   protected Maybe<AlternateWriter> _alternateWriter = nil;
   protected bool showToGo;
   protected Maybe<string> _title = nil;
   protected UiActionButtonType buttonType = UiActionButtonType.Normal;
   protected StatusType status = StatusType.None;
   protected int statusAlpha = 255;
   protected Timer statusTimer = new()
   {
      Interval = 100,
      Enabled = false
   };
   protected Maybe<BusyTextProcessor> _statusBusyProcessor = nil;
   protected Fader fader;
   protected Maybe<PieProgressProcessor> _pieProgressProcessor = nil;
   protected bool locked;
   protected DividerValidation dividerValidation = new DividerValidation.None();

   public event EventHandler<AutomaticMessageArgs>? AutomaticMessage;
   public event EventHandler<PaintEventArgs>? Painting;
   public event EventHandler<PaintEventArgs>? PaintingBackground;
   public event EventHandler<InitializeArgs>? Initialize;
   public event EventHandler<ArgumentsArgs>? Arguments;
   public event DoWorkEventHandler? DoWork;
   public event ProgressChangedEventHandler? ProgressChanged;
   public event RunWorkerCompletedEventHandler? RunWorkerCompleted;
   public event EventHandler? Tick;
   public event EventHandler<ValidatedArgs>? ValidateText;
   public event EventHandler<CheckStyleChangedArgs>? CheckStyleChanged;
   public event EventHandler<AppearanceOverrideArgs>? AppearanceOverride;
   public new event EventHandler? TextChanged;
   public event EventHandler<MessageShownArgs>? MessageShown;
   public event EventHandler<DrawToolTipEventArgs>? PaintToolTip;
   public event EventHandler<UiActionRectangleArgs>? ClickOnRectangle;
   public event EventHandler<UiActionRectangleArgs>? MouseMoveOnRectangle;
   public event EventHandler<UiActionRectanglePaintArgs>? PaintOnRectangle;
   public event EventHandler<UiActionAlternateArgs>? ClickOnAlternate;
   public event EventHandler<UiActionAlternateArgs>? DeleteOnAlternate;
   public event EventHandler<DynamicToolTipArgs>? DynamicToolTip;
   public event EventHandler<ChosenArgs>? ChosenItemSelected;
   public event EventHandler<ChosenArgs>? ChosenItemChecked;
   public event EventHandler<EventArgs>? ChooserOpened;
   public event EventHandler<EventArgs>? ChooserClosed;
   public event EventHandler<EventArgs>? StatusFaded;

   public UiAction()
   {
      SetStyle(ControlStyles.UserPaint, true);
      SetStyle(ControlStyles.DoubleBuffer, true);
      SetStyle(ControlStyles.AllPaintingInWmPaint, true);

      timerPaint.Tick += (_, _) =>
      {
         if (Enabled)
         {
            switch (type)
            {
               case UiActionType.BusyText when _busyTextProcessor is (true, var busyTextProcessor):
                  busyTextProcessor.OnTick();
                  break;
               case UiActionType.Automatic:
               {
                  var args = new AutomaticMessageArgs();
                  AutomaticMessage?.Invoke(this, args);
                  var _automaticText = args.GetText();
                  if (_automaticText)
                  {
                     Text = _automaticText;
                  }

                  break;
               }
               case UiActionType.Busy or UiActionType.ProgressIndefinite when _busyProcessor is (true, var busyProcessor):
                  busyProcessor.Advance();
                  break;
            }
         }

         this.Do(Refresh);
      };

      timer.Tick += (_, _) =>
      {
         Tick?.Invoke(this, EventArgs.Empty);
         if (oneTimeTimer)
         {
            timer.Enabled = false;
            oneTimeTimer = false;
         }
      };

      toolTip = new UiToolTip(this, UseEmojis);
      toolTip.SetToolTip(this, "");
      toolTip.Font = font;

      Resize += (_, _) =>
      {
         _busyTextProcessor.Reset();
         _progressDefiniteProcessor.Reset();
         _busyProcessor.Reset();

         switch (type)
         {
            case UiActionType.CheckBox:
               setUpCheckBox(text, BoxChecked);
               break;
            case UiActionType.Alternate:
               RectangleCount = Alternates.Length;
               break;
         }

         refresh();
      };

      Click += (_, _) =>
      {
         if (clickToCancel && !cancelled)
         {
            cancelled = true;
         }
      };

      Click += (_, _) =>
      {
         var location = PointToClient(Cursor.Position);

         foreach (var subText in subTexts.Values)
         {
            if (subText is ClickableSubText clickableSubText)
            {
               using var g = CreateGraphics();
               if (clickableSubText.Contains(g, location))
               {
                  clickableSubText.RaiseClick();
                  return;
               }
            }
         }

         switch (type)
         {
            case UiActionType.Alternate:
            {
               for (var i = 0; i < rectangles.Length; i++)
               {
                  if (rectangles[i].Contains(location) && DisabledIndex != i)
                  {
                     ClickOnRectangle?.Invoke(this, new UiActionRectangleArgs(i, location));

                     if (_alternateWriter is (true, var alternateWriter))
                     {
                        alternateWriter.SelectedIndex = i;
                        refresh();
                        ClickOnAlternate?.Invoke(this, new UiActionAlternateArgs(i, location, alternateWriter.Alternate, true));

                        if (_alternateWriter is (true, DeletableWriter deletableWriter))
                        {
                           var deletableRectangle = deletableWriter.DeletableRectangles[i];
                           if (deletableRectangle.Contains(location))
                           {
                              DeleteOnAlternate?.Invoke(this, new UiActionAlternateArgs(i, location, alternateWriter.Alternate, true));
                           }
                        }
                     }

                     return;
                  }
               }

               break;
            }
            case UiActionType.CheckBox when _alternateWriter is (true, CheckBoxWriter checkBoxWriter):
            {
               checkBoxWriter.BoxChecked = !checkBoxWriter.BoxChecked;
               break;
            }
         }
      };

      MouseMove += (_, _) =>
      {
         UpdateDynamicToolTip();

         var location = PointToClient(Cursor.Position);
         var invoked = false;

         for (var i = 0; i < rectangles.Length; i++)
         {
            if (!invoked && rectangles[i].Contains(location))
            {
               MouseMoveOnRectangle?.Invoke(this, new UiActionRectangleArgs(i, location));
               invoked = true;

               if (_alternateWriter is (true, var alternateWriter) && DisabledIndex != i)
               {
                  var color = alternateWriter.GetAlternateForeColor(i);
                  using var pen = new Pen(color);
                  pen.DashStyle = DashStyle.Dot;
                  var rectangle = rectangles[i].Shrink(2);
                  using var g = CreateGraphics();
                  g.DrawRectangle(pen, rectangle);

                  if (_alternateWriter is (true, DeletableWriter deletableWriter))
                  {
                     var deletableRectangle = deletableWriter.DeletableRectangles[i];
                     if (deletableRectangle.Contains(location))
                     {
                        deletableWriter.DrawBoldDeletable(g, i);
                     }
                  }
                  else
                  {
                     refresh();
                  }
               }
            }
         }

         foreach (var subText in subTexts.Values)
         {
            if (subText is ClickableSubText clickableSubText)
            {
               using var g = CreateGraphics();
               clickableSubText.DrawFocus(g, getForeColor(), location);
            }
         }
      };

      Resize += (_, _) => determineFloorAndCeiling();

      backgroundWorker = new Lazy<BackgroundWorker>(() =>
      {
         var worker = new BackgroundWorker { WorkerSupportsCancellation = true };
         worker.DoWork += (_, e) => DoWork?.Invoke(this, e);
         worker.ProgressChanged += (_, e) => ProgressChanged?.Invoke(this, e);
         worker.RunWorkerCompleted += (_, e) => RunWorkerCompleted?.Invoke(this, e);

         return worker;
      });

      workingTimer.Tick += (_, _) =>
      {
         (_, _working) = _working.Create(getWorking);

         this.Do(Refresh);
      };

      scroller = new Lazy<UiActionScroller>(() => new UiActionScroller(marqueeFont.Value, getClientRectangle(), getForeColor(), getBackColor()));

      statusTimer.Tick += (_, _) =>
      {
         if (status is not StatusType.Busy and not StatusType.Progress and not StatusType.ProgressStep)
         {
            statusAlpha -= 5;
            if (statusAlpha <= 0)
            {
               status = StatusType.None;
               statusTimer.Enabled = false;
               _successToolTip = nil;
               _failureToolTip = nil;
               _exceptionToolTip = nil;
               toolTip.ToolTipBox = false;
               StatusFaded?.Invoke(this, EventArgs.Empty);
            }
         }

         if (status is not StatusType.None)
         {
            this.Do(Refresh);
         }
      };

      fader = new Fader(this);
      fader.FadeComplete += (_, _) => fader.ClearTransparentLayeredWindow();
   }

   [Obsolete("Use ctor()")]
   public UiAction(Control control) : this()
   {
      control.Controls.Add(this);
      control.Resize += (_, _) => Refresh();
   }

   public bool AutoSizeText { get; set; }

   public bool Locked
   {
      get => locked;
      set
      {
         locked = value;
         Enabled = !value;
      }
   }

   protected static BusyProcessor getBusyProcessor(Rectangle clientRectangle) => busyStyle switch
   {
      BusyStyle.Default => new DefaultBusyProcessor(clientRectangle),
      BusyStyle.Sine => new SineBusyProcessor(clientRectangle),
      BusyStyle.Rectangle => new RectangleBusyProcessor(clientRectangle),
      BusyStyle.BarberPole => new BarberPoleBusyProcessor(clientRectangle),
      _ => new DefaultBusyProcessor(clientRectangle)
   };

   protected void activateProcessor(Graphics graphics)
   {
      switch (type)
      {
         case UiActionType.BusyText:
         {
            var clientRectangle = getClientRectangle();
            _busyTextProcessor.Activate(() => new BusyTextProcessor(Color.White, clientRectangle));
            break;
         }
         case UiActionType.Busy:
         {
            var clientRectangle = getClientRectangle();
            _busyProcessor.Activate(() => getBusyProcessor(clientRectangle));
            break;
         }
         case UiActionType.ProgressDefinite:
         {
            var clientRectangle = getClientRectangle();
            var _uiAction = maybe<UiAction>() & showToGo & this;
            _progressDefiniteProcessor.Activate(() => new ProgressDefiniteProcessor(font, graphics, clientRectangle, _uiAction, UseEmojis));
            break;
         }
         case UiActionType.ProgressMini:
         {
            var clientRectangle = getClientRectangle();
            _progressMiniProcessor.Activate(() => new ProgressMiniProcessor(clientRectangle));
            break;
         }
      }
   }

   public Guid Id => id;

   public bool Cancelled
   {
      get => cancelled;
      set => cancelled = value;
   }

   protected SubText getWorking()
   {
      var workingText = _workingText | "working";
      return new SubText(this, workingText, 0, 0, ClientSize, ClickGlyph, ChooserGlyph).Set.MiniInverted(workingAlignment, false, false).SubText;
   }

   public UiActionType Type
   {
      get => type;
      set
      {
         type = value;
         _symbolWriter = nil;
         refresh();
      }
   }

   public bool ClickToCancel
   {
      get => clickToCancel;
      set
      {
         clickToCancel = value;
         refresh();
      }
   }

   public bool TaskBarProgress { get; set; }

   public bool Checked
   {
      get => CheckStyle == CheckStyle.Checked;
      set => CheckStyle = value ? CheckStyle.Checked : CheckStyle.Unchecked;
   }

   public CheckStyle CheckStyle
   {
      get => checkStyle;
      set
      {
         checkStyle = value;
         CheckStyleChanged?.Invoke(this, new CheckStyleChangedArgs(id, checkStyle));
         determineFloorAndCeiling();
         Refresh();
      }
   }

   public CardinalAlignment MessageAlignment { get; set; } = CardinalAlignment.Center;

   public bool IsPath { get; set; }

   internal void SetCheckStyle(CheckStyle checkStyle)
   {
      this.checkStyle = checkStyle;
      Refresh();
   }

   public void SetForeColor(Color foreColor) => _foreColor = foreColor;

   public void SetBackColor(Color backColor) => _backColor = backColor;

   public void SetStyle(MessageStyle style) => _style = style;

   public override Font? Font
   {
#pragma warning disable CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
      get => font;
#pragma warning restore CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
      set
      {
         if (value is not null)
         {
            font = value;
            italicFont = new Font(font, FontStyle.Italic);
            boldFont = new Font(font, FontStyle.Bold);
            italicBoldFont = new Font(font, FontStyle.Italic | FontStyle.Bold);
            toolTip.Font = font;
         }
      }
   }

   public Font NonNullFont
   {
      get => font;
      set
      {
         font = value;
         italicFont = new Font(font, FontStyle.Italic);
         boldFont = new Font(font, FontStyle.Bold);
         italicBoldFont = new Font(font, FontStyle.Italic | FontStyle.Bold);
         toolTip.Font = font;
      }
   }

   protected string withEmojis(string text) => UseEmojis ? text.EmojiSubstitutions() : text;

   public Maybe<string> Title
   {
      get => _title;
      set => _title = value.Map(withEmojis);
   }

   protected void setToolTip()
   {
      if (PaintToolTip is not null)
      {
         toolTip.Action = action<object, DrawToolTipEventArgs>((_, e) => PaintToolTip.Invoke(this, e));
      }
      else if (DynamicToolTip is not null)
      {
         var args = new DynamicToolTipArgs(CurrentPositionIndex);
         DynamicToolTip?.Invoke(this, args);
         if (args.ToolTipText is (true, var toolTipText))
         {
            if (!toolTip.Action)
            {
               _oldTitle = toolTip.ToolTipTitle.NotEmpty();
            }

            toolTip.ToolTipTitle = "";
            toolTip.Text = toolTipText;
            toolTip.Action = action<object, DrawToolTipEventArgs>((_, e) =>
            {
               toolTip.DrawTextInRectangle(e.Graphics, toolTip.Font, Color.White, Color.CadetBlue, e.Bounds);
            });
         }
      }
      else if (_successToolTip is (true, var successToolTip))
      {
         if (!toolTip.Action)
         {
            _oldTitle = toolTip.ToolTipTitle.NotEmpty();
         }

         toolTip.ToolTipTitle = "success";
         toolTip.Text = successToolTip;
         toolTip.Action = action<object, DrawToolTipEventArgs>((_, e) =>
         {
            toolTip.DrawTextInRectangle(e.Graphics, toolTip.Font, Color.White, Color.Green, e.Bounds);
            toolTip.DrawTitle(e.Graphics, toolTip.Font, Color.Green, Color.White, e.Bounds);
         });
         this.Do(() => toolTip.SetToolTip(this, successToolTip));
      }
      else if (_failureToolTip is (true, var failureToolTip))
      {
         if (!toolTip.Action)
         {
            _oldTitle = toolTip.ToolTipTitle.NotEmpty();
         }

         toolTip.ToolTipTitle = "failure";
         toolTip.Text = failureToolTip;
         toolTip.Action = action<object, DrawToolTipEventArgs>((_, e) =>
         {
            toolTip.DrawTextInRectangle(e.Graphics, toolTip.Font, Color.Black, Color.Gold, e.Bounds);
            toolTip.DrawTitle(e.Graphics, toolTip.Font, Color.Gold, Color.Black, e.Bounds);
         });
         this.Do(() => toolTip.SetToolTip(this, failureToolTip));
      }
      else if (_exceptionToolTip is (true, var exceptionToolTip))
      {
         if (!toolTip.Action)
         {
            _oldTitle = toolTip.ToolTipTitle.NotEmpty();
         }

         toolTip.ToolTipTitle = "exception";
         toolTip.Text = exceptionToolTip;
         toolTip.Action = action<object, DrawToolTipEventArgs>((_, e) =>
         {
            toolTip.DrawTextInRectangle(e.Graphics, toolTip.Font, Color.White, Color.Red, e.Bounds);
            toolTip.DrawTitle(e.Graphics, toolTip.Font, Color.Red, Color.White, e.Bounds);
         });
         this.Do(() => toolTip.SetToolTip(this, exceptionToolTip));
      }
      else if (_noStatusToolTip is (true, var noStatusToolTip))
      {
         if (!toolTip.Action)
         {
            _oldTitle = toolTip.ToolTipTitle.NotEmpty();
         }

         toolTip.ToolTipTitle = "no status";
         toolTip.Text = noStatusToolTip;
         toolTip.Action = action<object, DrawToolTipEventArgs>((_, e) =>
         {
            toolTip.DrawTextInRectangle(e.Graphics, toolTip.Font, Color.Black, Color.White, e.Bounds);
            toolTip.DrawTitle(e.Graphics, toolTip.Font, Color.White, Color.Black, e.Bounds);
         });
         this.Do(() => toolTip.SetToolTip(this, noStatusToolTip));
      }
      else
      {
         switch (type)
         {
            case UiActionType.Failure:
            {
               if (!toolTip.Action)
               {
                  _oldTitle = toolTip.ToolTipTitle.NotEmpty();
               }

               toolTip.ToolTipTitle = "failure";
               toolTip.Text = text;
               toolTip.Action = action<object, DrawToolTipEventArgs>((_, e) =>
               {
                  toolTip.DrawTextInRectangle(e.Graphics, toolTip.Font, Color.Black, Color.Gold, e.Bounds);
                  toolTip.DrawTitle(e.Graphics, toolTip.Font, Color.Gold, Color.Black, e.Bounds);
               });
               this.Do(() => toolTip.SetToolTip(this, text));
               break;
            }
            case UiActionType.Exception:
            {
               if (!toolTip.Action)
               {
                  _oldTitle = toolTip.ToolTipTitle.NotEmpty();
               }

               toolTip.ToolTipTitle = "exception";
               toolTip.Text = text;
               toolTip.Action = action<object, DrawToolTipEventArgs>((_, e) =>
               {
                  toolTip.DrawTextInRectangle(e.Graphics, toolTip.Font, Color.White, Color.Red, e.Bounds);
                  toolTip.DrawTitle(e.Graphics, toolTip.Font, Color.Red, Color.White, e.Bounds);
               });
               this.Do(() => toolTip.SetToolTip(this, text));
               break;
            }
            case UiActionType.NoStatus:
            {
               if (!toolTip.Action)
               {
                  _oldTitle = toolTip.ToolTipTitle.NotEmpty();
               }

               toolTip.ToolTipTitle = "no status";
               toolTip.Text = text;
               toolTip.Action = action<object, DrawToolTipEventArgs>((_, e) =>
               {
                  toolTip.DrawTextInRectangle(e.Graphics, toolTip.Font, Color.Black, Color.White, e.Bounds);
                  toolTip.DrawTitle(e.Graphics, toolTip.Font, Color.White, Color.Black, e.Bounds);
               });
               this.Do(() => toolTip.SetToolTip(this, text));
               break;
            }
            default:
            {
               if (Clickable && ClickText.IsNotEmpty())
               {
                  if (_oldTitle is (true, var oldTitle))
                  {
                     toolTip.ToolTipTitle = oldTitle;
                     _oldTitle = nil;
                  }
                  else
                  {
                     toolTip.ToolTipTitle = "";
                  }

                  toolTip.Text = ClickText;
                  toolTip.Action = nil;
                  this.Do(() => toolTip.SetToolTip(this, ClickText));
               }
               else
               {
                  if (_oldTitle is (true, var oldTitle))
                  {
                     toolTip.ToolTipTitle = oldTitle;
                     _oldTitle = nil;
                  }
                  else
                  {
                     toolTip.ToolTipTitle = "";
                  }

                  toolTip.Text = text;
                  toolTip.Action = nil;
                  this.Do(() => toolTip.SetToolTip(this, text));
               }

               break;
            }
         }
      }

      refresh();
   }

   public override string? Text
   {
#pragma warning disable CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
      get
#pragma warning restore CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
      {
         return text;
      }
      set
      {
         text = value ?? "";
         this.Do(setToolTip);
         TextChanged?.Invoke(this, EventArgs.Empty);
      }
   }

   public string NonNullText
   {
      get => text;
      set
      {
         text = value;
         this.Do(setToolTip);
         TextChanged?.Invoke(this, EventArgs.Empty);
      }
   }

   public bool Is3D { get; set; }

   public AutoHash<UiActionType, Color> ForeColors => foreColors;

   public AutoHash<UiActionType, Color> BackColors => backColors;

   public AutoHash<UiActionType, MessageStyle> Styles => styles;

   public Image Image
   {
      set
      {
         _image = value;
         refresh();
      }
   }

   public void ClearImage()
   {
      _image = nil;
      refresh();
   }

   public bool StretchImage { get; set; }

   public CardinalAlignment CardinalAlignment { get; set; } = CardinalAlignment.Center;

   protected Font getFont() => getStyle() switch
   {
      MessageStyle.None => font,
      MessageStyle.Italic => italicFont,
      MessageStyle.Bold => boldFont,
      MessageStyle.ItalicBold => italicBoldFont,
      _ => font
   };

   protected void refresh()
   {
      this.Do(() =>
      {
         determineFloorAndCeiling();
         Invalidate();
         Update();
      });
   }

   public void ShowMessage(string message, UiActionType type)
   {
      FloatingException(false);
      Busy(false);
      this.type = type;
      Text = message;

      MessageShown?.Invoke(this, new MessageShownArgs(text, this.type));

      if (type == UiActionType.Http)
      {
         if (!httpHandlerAdded)
         {
            Click += openUrl;
            httpHandlerAdded = true;
         }
      }
      else if (httpHandlerAdded)
      {
         Click -= openUrl;
         httpHandlerAdded = false;
      }

      if (_taskBarProgress is (true, var taskBarProgress))
      {
         taskBarProgress.State = WinForms.Controls.TaskBarProgress.TaskBarState.NoProgress;
      }

      _taskBarProgress = nil;
      _alternateWriter = nil;

      refresh();
   }

   public void Display(string message, Color foreColor, Color backColor)
   {
      ForeColor = foreColor;
      BackColor = backColor;
      ShowMessage(message, UiActionType.Display);
   }

   public void Uninitialized(string message) => ShowMessage(message, UiActionType.Uninitialized);

   public void Message(string message) => ShowMessage(message, UiActionType.Message);

   public void Exception(Exception exception)
   {
      ShowMessage(exception.Message, UiActionType.Exception);
   }

   public void Success(string message) => ShowMessage(message, UiActionType.Success);

   public void Failure(string message) => ShowMessage(message, UiActionType.Failure);

   public void NoStatus(string message) => ShowMessage(message, UiActionType.NoStatus);

   public void Caution(string message) => ShowMessage(message, UiActionType.Caution);

   public void Selected(string message) => ShowMessage(message, UiActionType.Selected);

   public void Unselected(string message) => ShowMessage(message, UiActionType.Unselected);

   public void FileName(FileName file, bool checkForFileExistence = true, bool isFailure = false)
   {
      try
      {
         IsPath = true;
         if (isFailure)
         {
            Failure(file.FullPath);
         }
         else if (checkForFileExistence)
         {
            if (file)
            {
               Success(file.FullPath);
            }
            else
            {
               Failure(file.FullPath);
            }
         }
         else
         {
            Message(file.FullPath);
         }
      }
      catch (Exception exception)
      {
         Exception(exception);
      }
   }

   public void FolderName(FolderName folder, bool checkForFolderExistence = true, bool isFailure = false)
   {
      try
      {
         IsPath = true;
         if (isFailure)
         {
            Failure(folder.FullPath);
         }
         else if (checkForFolderExistence)
         {
            if (folder)
            {
               Success(folder.FullPath);
            }
            else
            {
               Failure(folder.FullPath);
            }
         }
         else
         {
            Message(folder.FullPath);
         }
      }
      catch (Exception exception)
      {
         Exception(exception);
      }
   }

   public void Tape()
   {
      type = UiActionType.Tape;
      refresh();
   }

   public void ProgressText(string text)
   {
      Text = text;
      type = UiActionType.ProgressIndefinite;

      refresh();
   }

   public void Result(Result<(string, UiActionType)> _result)
   {
      if (_result is (true, var (message, messageProgressType)))
      {
         ShowMessage(message, messageProgressType);
      }
      else
      {
         Exception(_result.Exception);
      }
   }

   public void Result(Result<string> _result)
   {
      if (_result)
      {
         Success(_result);
      }
      else
      {
         Exception(_result.Exception);
      }
   }

   public void Result(Result<string> _result, UiActionType type)
   {
      if (_result is (true, var message))
      {
         ShowMessage(message, type);
      }
      else
      {
         Exception(_result.Exception);
      }
   }

   public void Optional(Optional<string> _message, string nilMessage)
   {
      if (_message is (true, var message))
      {
         Success(message);
      }
      else if (_message.Exception is (true, var exception))
      {
         Exception(exception);
      }
      else
      {
         Failure(nilMessage);
      }
   }

   public void Optional(Optional<(string message, UiActionType type)> _message, string nilMessage)
   {
      if (_message is (true, var (message, uiActionType)))
      {
         ShowMessage(message, uiActionType);
      }
      else if (_message.Exception is (true, var exception))
      {
         Exception(exception);
      }
      else
      {
         Failure(nilMessage);
      }
   }

   public void Optional(Optional<(string message, UiActionType type)> _message, Func<string> nilMessage)
   {
      if (_message is (true, var (message, uiActionType)))
      {
         ShowMessage(message, uiActionType);
      }
      else if (_message.Exception is (true, var exception))
      {
         Exception(exception);
      }
      else
      {
         Failure(nilMessage());
      }
   }

   public void Optional(Optional<string> _message, UiActionType type, string nilMessage)
   {
      if (_message is (true, var message))
      {
         ShowMessage(message, type);
      }
      else if (_message.Exception is (true, var exception))
      {
         Exception(exception);
      }
      else
      {
         Failure(nilMessage);
      }
   }

   public void Optional(Optional<string> _message, UiActionType type, Func<string> nilMessage)
   {
      if (_message is (true, var message))
      {
         ShowMessage(message, type);
      }
      else if (_message.Exception is (true, var exception))
      {
         Exception(exception);
      }
      else
      {
         Failure(nilMessage());
      }
   }

   public void Optional(Optional<string> _message, Func<string> nilMessageFunc)
   {
      if (_message is (true, var message))
      {
         Success(message);
      }
      else if (_message.Exception is (true, var exception))
      {
         Exception(exception);
      }
      else
      {
         Failure(nilMessageFunc());
      }
   }

   public void Optional(Optional<string> _message)
   {
      if (_message is (true, var message))
      {
         Success(message);
      }
      else if (_message.Exception is (true, var exception))
      {
         Exception(exception);
      }
   }

   public void AttachTo(string text, Control control, string fontName = "Segoe UI", float fontSize = 9, int left = -1, bool stretch = false,
      int width = -1)
   {
      this.text = text;
      type = UiActionType.ControlLabel;

      MessageShown?.Invoke(this, new MessageShownArgs(this.text, type));

      control.Move += (_, _) =>
      {
         Location = new Point(control.Left, control.Top - Height + 1);
         Refresh();
      };

      using var attachedFont = new Font(fontName, fontSize);
      var size = TextRenderer.MeasureText(this.text, attachedFont);

      if (left == -1)
      {
         left = control.Left;
      }

      if (width == -1)
      {
         width = stretch ? control.Width - left : size.Width + 20;
      }

      this.SetUp(left, control.Top - size.Height - 3, width, size.Height + 4, fontName, fontSize);

      Refresh();
   }

   public bool Clickable => _clickText || HasDynamicToolTip;

   public bool HasDynamicToolTip => DynamicToolTip is not null;

   public Maybe<string> RaiseDynamicToolTip()
   {
      if (DynamicToolTip is not null)
      {
         var args = new DynamicToolTipArgs(CurrentPositionIndex);
         DynamicToolTip.Invoke(this, args);
         return args.ToolTipText;
      }
      else
      {
         return nil;
      }
   }

   public void UpdateDynamicToolTip()
   {
      if (DynamicToolTip is not null)
      {
         setToolTip();
      }
   }

   public string ClickText
   {
      get => _clickText | text;
      set
      {
         _clickText = maybe<string>() & value.IsNotEmpty() & value;
         this.Do(setToolTip);
      }
   }

   public int Minimum { get; set; } = 1;

   protected Optional<IntPtr> getHandle()
   {
      try
      {
         if (MainForm is (true, var mainForm))
         {
            return mainForm.Get(() => mainForm.Handle);
         }
         else if (ParentForm is null)
         {
            return nil;
         }
         else
         {
            return ParentForm.Get(() => ParentForm.Handle);
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   protected Optional<TaskBarProgress> getTaskBarProgress(int value) => getHandle().Map(h => new TaskBarProgress(h, value));

   protected Optional<TaskBarProgress> getTaskBarProgress()
   {
      return getTaskBarProgress(0).OnJust(t => t.State = WinForms.Controls.TaskBarProgress.TaskBarState.Indeterminate);
   }

   public int Maximum
   {
      get => maximum;
      set
      {
         maximum = value;
         index = Minimum;
         if (TaskBarProgress && getTaskBarProgress(value) is (true, var taskBarProgress))
         {
            _taskBarProgress = taskBarProgress;
         }
         else
         {
            _taskBarProgress = nil;
         }
      }
   }

   public bool Stopwatch { get; set; }

   public bool StopwatchInverted { get; set; } = true;

   public bool ShowToGo
   {
      get => showToGo;
      set => showToGo = value;
   }

   public Maybe<TimeSpan> Elapsed => maybe<TimeSpan>() & Stopwatch & (() => stopwatch.Value.Elapsed);

   public void Progress(int value, string text = "", bool asPercentage = false)
   {
      if (asPercentage)
      {
         _percentage = value;
      }
      else
      {
         this.value = value;
      }

      Text = text;
      type = UiActionType.ProgressDefinite;

      MessageShown?.Invoke(this, new MessageShownArgs(Text, type));

      if (_taskBarProgress is (true, var taskBarProgress))
      {
         taskBarProgress.Value = value;
      }

      refresh();
   }

   public void Progress(string text)
   {
      if (_progressDefiniteProcessor is (true, var processor))
      {
         processor.ShowToGo = showToGo;
      }

      value = index++;

      Text = text;
      type = UiActionType.ProgressDefinite;

      MessageShown?.Invoke(this, new MessageShownArgs(Text, type));

      if (_taskBarProgress is (true, var taskBarProgress))
      {
         taskBarProgress.Value = value;
      }

      refresh();
   }

   public void ProgressMini()
   {
      value = index++;
      Text = "";
      type = UiActionType.ProgressMini;

      if (_taskBarProgress is (true, var taskBarProgress))
      {
         taskBarProgress.Value = value;
      }

      refresh();
   }

   public bool ProgressStripe { get; set; }

   public void Progress()
   {
      value = index++;
      refresh();
   }

   public void Progress(int percentage)
   {
      _percentage = percentage;

      EmptyTextTitle = nil;
      Text = "";
      type = UiActionType.MuteProgress;

      MessageShown?.Invoke(this, new MessageShownArgs("", type));

      if (_taskBarProgress is (true, var taskBarProgress))
      {
         taskBarProgress.Value = _percentage | 0;
      }

      refresh();
   }

   public int ProgressIndex => index;

   public void StartStopwatch() => stopwatch.Value.Start();

   public void StopStopwatch() => stopwatch.Value.Stop();

   public void ResetStopwatch() => stopwatch.Value.Reset();

   public void Busy(string text)
   {
      Text = text;
      type = UiActionType.BusyText;

      MessageShown?.Invoke(this, new MessageShownArgs(Text, type));

      if (TaskBarProgress && !_taskBarProgress)
      {
         _taskBarProgress = getTaskBarProgress();
      }

      this.Do(() => timerPaint.Enabled = true);
      refresh();
   }

   protected Color getForeColor(UiActionType type) => foreColors[type];

   protected Color getForeColor(StatusType type) => type switch
   {
      StatusType.None => getForeColor(),
      StatusType.Busy => getForeColor(UiActionType.Busy),
      StatusType.Success => getForeColor(UiActionType.Success),
      StatusType.Failure => getForeColor(UiActionType.Failure),
      StatusType.Exception => getForeColor(UiActionType.Exception),
      StatusType.Done => getForeColor(UiActionType.Done),
      StatusType.Progress => getForeColor(UiActionType.ProgressDefinite),
      StatusType.ProgressStep => getForeColor(UiActionType.ProgressDefinite),
      _ => getForeColor()
   };

   protected Color getBackColor(StatusType type) => type switch
   {
      StatusType.None => getBackColor(),
      StatusType.Busy => getBackColor(UiActionType.Busy),
      StatusType.Success => getBackColor(UiActionType.Success),
      StatusType.Failure => getBackColor(UiActionType.Failure),
      StatusType.Exception => getBackColor(UiActionType.Exception),
      StatusType.Done => getBackColor(UiActionType.Done),
      StatusType.Progress => getBackColor(UiActionType.ProgressDefinite),
      StatusType.ProgressStep => getBackColor(UiActionType.ProgressDefinite),
      _ => getBackColor()
   };

   protected Color getForeColor() => type == UiActionType.Display ? ForeColor : _foreColor | (() => foreColors[type]);

   protected Color getBackColor(UiActionType type) => backColors[type];

   protected Color getBackColor() => type == UiActionType.Display ? BackColor : _backColor | (() => backColors[type]);

   protected MessageStyle getStyle(UiActionType type) => styles[type];

   protected MessageStyle getStyle() => _style | (() => styles[type]);

   protected Rectangle getClientRectangle()
   {
      Rectangle rectangle;
      if (Arrow)
      {
         var arrowSection = (int)(ClientRectangle.Width * START_AMOUNT);
         var remainder = ClientRectangle.Width - arrowSection;
         rectangle = ClientRectangle with { X = Width - arrowSection, Width = Width - 2 * remainder };
      }
      else
      {
         rectangle = ClientRectangle;
      }

      return isMouseDown() ? rectangle.Reposition(1, 1).Resize(-2, -2) : rectangle;
   }

   public bool ClickGlyph { get; set; } = true;

   public bool ChooserGlyph { get; set; }

   protected override void OnPaint(PaintEventArgs e)
   {
      base.OnPaint(e);

      if (!Enabled && !_symbolWriter && !locked)
      {
         var disabledWriter = DisabledWriter.FromUiAction(this, UseEmojis);

         disabledWriter.Write(e.Graphics, text, true);

         if (ProgressStripe && value < maximum)
         {
            var clientRectangleWidth = ClientRectangle.Width;
            var percentage = getPercentage(clientRectangleWidth);
            var top = ClientRectangle.Bottom - 4;
            var remainder = clientRectangleWidth - percentage;
            drawLine(e.Graphics, Color.Black, ((ClientRectangle.Left + percentage, top), (remainder, 0)));
         }

         return;
      }

      if (locked)
      {
         using var measureFont = new Font("Consolas", 20f, FontStyle.Regular);
         var size = UiActionWriter.TextSize(e.Graphics, "/big-x", measureFont, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, UseEmojis);
         var rectangle = size.West(getClientRectangle());
         var lockedWriter = new UiActionWriter(rectangle, measureFont, Color.White);
         lockedWriter.Write(e.Graphics, "/big-x", type is UiActionType.NoStatus);
      }

      if (status is StatusType.Progress or StatusType.ProgressStep && _pieProgressProcessor is (true, var pieProgressProcessor))
      {
         pieProgressProcessor.OnPaint(e.Graphics);
      }

      if (PaintOnRectangle is not null && rectangles.Length > 0)
      {
         for (var i = 0; i < rectangles.Length; i++)
         {
            PaintOnRectangle.Invoke(this, new UiActionRectanglePaintArgs(e.Graphics, i));
         }

         return;
      }

      activateProcessor(e.Graphics);

      var clientRectangle = getClientRectangle();

      determineFloorAndCeiling();

      var writer = new UiActionWriter(MessageAlignment, AutoSizeText, _floor, _ceiling, buttonType, UseEmojis)
      {
         Rectangle = glyphAdjustedClientRectangle(),
         Font = getFont(),
         Color = getForeColor(),
         CheckStyle = checkStyle,
         EmptyTextTitle = EmptyTextTitle,
         IsPath = IsPath,
         Required = Required
      };
      var httpWriter = new Lazy<HttpWriter>(() => new HttpWriter(text, glyphAdjustedClientRectangle(), getFont()));

      switch (type)
      {
         case UiActionType.ProgressIndefinite:
            writer.Write(e.Graphics, text, false);
            break;
         case UiActionType.Busy when FlipFlop:
         {
            var foreColor = flipOn ? Color.White : Color.Black;
            var backColor = flipOn ? Color.Black : Color.White;
            var flipFlop = SubText("starting").Set.ForeColor(foreColor).BackColor(backColor).GoToUpperLeft(2).SubText;
            if (_flipFlop is (true, var oldFlipFlop))
            {
               RemoveSubText(oldFlipFlop);
               _flipFlop = flipFlop;
            }

            flipFlop.Draw(e.Graphics);

            flipOn = !flipOn;

            break;
         }
         case UiActionType.Busy when _busyProcessor is (true, var busyProcessor):
            busyProcessor.OnPaint(e.Graphics);
            break;
         case UiActionType.ProgressDefinite when _progressDefiniteProcessor is (true, var progressDefiniteProcessor):
         {
            var autoSize = writer.AutoSizeText;
            writer.AutoSizeText = false;
            var percentage = getPercentage();
            var percentText = $"{percentage}%";
            writer.Rectangle = progressDefiniteProcessor.PercentRectangle;
            writer.Center(true);
            writer.Color = Color.Black;
            writer.Write(e.Graphics, percentText, false);

            writer.Rectangle = progressDefiniteProcessor.TextRectangle;
            writer.Center(true);
            writer.Color = getForeColor();
            writer.Write(e.Graphics, text, false);

            if (_progressSubText is (true, var progressSubText))
            {
               progressSubText.Draw(e.Graphics);
            }

            progressDefiniteProcessor.OnPaint(e.Graphics, percentage, Color.Black, clientRectangle);

            writer.AutoSizeText = autoSize;

            break;
         }
         case UiActionType.MuteProgress:
         {
            var percentText = $"{getPercentage()}%";
            writer.Write(e.Graphics, percentText, false);

            if (_progressSubText is (true, var progressSubText))
            {
               progressSubText.Draw(e.Graphics);
            }

            break;
         }
         case UiActionType.BusyText when _busyTextProcessor is (true, var busyTextProcessor):
         {
            var allRectangle = writer.TextRectangle(text, e.Graphics, clientRectangle);
            var allX = allRectangle.X;
            var allY = allRectangle.Y;
            var drawRectangle = busyTextProcessor.DrawRectangle;
            var drawX = drawRectangle.X + drawRectangle.Width;
            var drawY = drawRectangle.Y + drawRectangle.Height;
            if (allX < drawX || allY < drawY)
            {
               allRectangle = busyTextProcessor.TextRectangle;
            }

            writer.Rectangle = allRectangle;
            writer.Center(true);
            writer.Write(e.Graphics, text, false);
            break;
         }
         case UiActionType.ControlLabel:
            writer.Write(e.Graphics, text, false);
            break;
         case UiActionType.Http:
         {
            httpWriter.Value.OnPaint(e.Graphics, isUrlGood);
            break;
         }
         case UiActionType.Console:
            scroller.Value.OnPaint(e.Graphics);
            break;
         case UiActionType.Display:
            writer.Color = ForeColor;
            writer.Write(e.Graphics, text, false);
            break;
         case UiActionType.Symbol when _symbolWriter is (true, var symbolWriter):
            symbolWriter.OnPaint(e.Graphics, clientRectangle, Enabled);
            break;
         case UiActionType.Button:
            writer.Write(e.Graphics, text, false);
            break;
         case UiActionType.Alternate when _alternateWriter is (true, var alternateWriter):
            alternateWriter.OnPaint(e.Graphics);
            break;
         case UiActionType.CheckBox when _alternateWriter is (true, var alternateWriter):
            alternateWriter.OnPaint(e.Graphics);
            break;
         case UiActionType.Divider:
         {
            var rectangle = getDividerRectangle();
            var dividerForeColor = getDividerForeColor();
            var dividerBackColor = getDividerBackColor();
            var _dividerText = getDividerValidationText();
            if (isDirty)
            {
               using var brush = new HatchBrush(HatchStyle.DiagonalCross, dividerBackColor, dividerForeColor);
               e.Graphics.FillRectangle(brush, rectangle);
            }
            else
            {
               using var brush = new SolidBrush(dividerBackColor);
               e.Graphics.FillRectangle(brush, rectangle);
            }

            if (_dividerText is (true, var dividerText))
            {
               var dividerWriter = new RectangleWriter(dividerText, rectangle) { FontSize = 8f, ForeColor = dividerForeColor };
               dividerWriter.Write(e.Graphics);
            }

            var textRectangle = getDividerTextRectangle(e.Graphics, clientRectangle);
            using var backBrush = new SolidBrush(Color.CadetBlue);
            fillRectangle(e.Graphics, backBrush, textRectangle);

            using var pen = new Pen(dividerForeColor);
            var flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis |
               TextFormatFlags.LeftAndRightPadding;
            TextRenderer.DrawText(e.Graphics, text, Font, textRectangle, Color.White, flags);
            break;
         }
         default:
         {
            if (type is not UiActionType.Tape)
            {
               writer.Write(e.Graphics, text, type is UiActionType.NoStatus);
            }

            break;
         }
      }

      drawStopwatch();

      var clickGlyphWidth = 0;

      if (Clickable)
      {
         var color = getForeColor();
         drawClickGlyph(e, clientRectangle, color);

         clickGlyphWidth = 4;

         if (mouseInside || mouseDown)
         {
            using var dashedPen = new Pen(color, 1);
            dashedPen.DashStyle = DashStyle.Dash;
            var rectangle = clientRectangle;
            rectangle.Inflate(-2, -2);
            drawRectangle(e.Graphics, dashedPen, rectangle);
         }
      }

      if (ProgressStripe && value < maximum)
      {
         var clientRectangleWidth = clientRectangle.Width - clickGlyphWidth;
         var percentage = getPercentage(clientRectangleWidth);
         var top = clientRectangle.Bottom - 4;
         var color = getBackColor();
         drawLine(e.Graphics, color, ((clientRectangle.Left, top), (percentage, 0)));

         var remainder = clientRectangleWidth - percentage;
         color = getForeColor();
         drawLine(e.Graphics, color, ((clientRectangle.Left + percentage, top), (remainder, 0)));
      }

      if (ShowFocus && Focused)
      {
         var color = getForeColor();
         using var dashedPen = new Pen(color, 2);
         dashedPen.DashStyle = DashStyle.Dot;
         var rectangle = clientRectangle;
         rectangle.Inflate(-8, -8);
         drawRectangle(e.Graphics, dashedPen, rectangle);
      }

      drawAllSubTexts(e.Graphics, type, clientRectangle);

      drawTitle(e.Graphics, clientRectangle);

      drawStatus(e.Graphics, clientRectangle);

      Painting?.Invoke(this, e);
      return;

      void drawStopwatch()
      {
         if (Stopwatch)
         {
            var elapsed = stopwatch.Value.Elapsed.ToString(@"mm\:ss");
            using var stopwatchFont = new Font("Consolas", 8);
            var size = TextRenderer.MeasureText(e.Graphics, elapsed, stopwatchFont);
            var location = new Point(clientRectangle.Width - size.Width - 8, 4);
            var rectangle = new Rectangle(location, size);
            if (StopwatchInverted)
            {
               var foreColor = getBackColor();
               var backColor = getForeColor();
               using var brush = new SolidBrush(backColor);
               e.Graphics.FillRectangle(brush, rectangle);
               TextRenderer.DrawText(e.Graphics, elapsed, stopwatchFont, rectangle, foreColor);
               using var pen = new Pen(foreColor);
               e.Graphics.DrawRectangle(pen, rectangle);
            }
            else
            {
               var foreColor = getForeColor();
               TextRenderer.DrawText(e.Graphics, elapsed, stopwatchFont, rectangle, foreColor);
               using var pen = new Pen(foreColor);
               e.Graphics.DrawRectangle(pen, rectangle);
            }
         }
      }

      Color getDividerForeColor() => dividerValidation is DividerValidation.Failure ? Color.Black : Color.White;

      Color getDividerBackColor() => dividerValidation switch
      {
         DividerValidation.Error => Color.Red,
         DividerValidation.Failure => Color.Yellow,
         DividerValidation.Invalid => Color.Maroon,
         DividerValidation.None => Color.DarkBlue,
         DividerValidation.Valid => Color.Green,
         _ => throw new ArgumentOutOfRangeException(nameof(dividerValidation))
      };

      Maybe<string> getDividerValidationText() => dividerValidation switch
      {
         DividerValidation.Error error => error.Exception.Message,
         DividerValidation.Failure failure => failure.Message,
         DividerValidation.Invalid invalid => invalid.Message,
         DividerValidation.None => nil,
         DividerValidation.Valid => nil,
         _ => nil
      };
   }

   protected void drawTitle(Graphics g, Rectangle clientRectangle)
   {
      if (_title is (true, var title))
      {
         var rectangle = AutoSizingWriter.NarrowRectangle(clientRectangle, _floor, _ceiling);
         var titleFont = new Font(font.FontFamily, 8, font.Style);
         var textFormatFlags = TextFormatFlags.EndEllipsis | TextFormatFlags.HidePrefix | TextFormatFlags.HorizontalCenter |
            TextFormatFlags.VerticalCenter;
         var size = UiActionWriter.TextSize(g, title, titleFont, textFormatFlags, UseEmojis);
         var margin = (rectangle.Width - size.Width) / 2;
         if (margin > 0)
         {
            var titleRectangle = rectangle with
            {
               X = rectangle.X + margin - 3,
               Height = size.Height + 2,
               Width = rectangle.Width - 2 * margin + 6
            };
            var foreColor = Color.Black;
            var backColor = Color.AntiqueWhite;

            using var brush = new SolidBrush(backColor);
            g.FillRectangle(brush, titleRectangle);

            using var pen = new Pen(foreColor, 2);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;

            var p1 = titleRectangle.NorthWest();
            var p2 = titleRectangle.SouthWest();

            g.DrawLine(pen, p1, p2);

            p1 = titleRectangle.NorthEast();
            p2 = titleRectangle.SouthEast();

            g.DrawLine(pen, p1, p2);

            p1 = titleRectangle.SouthWest();
            p2 = titleRectangle.SouthEast();

            g.DrawLine(pen, p1, p2);

            using var textBrush = new SolidBrush(foreColor);
            var stringFormat = new StringFormat(StringFormatFlags.NoWrap)
            {
               Alignment = StringAlignment.Center,
               LineAlignment = StringAlignment.Center
            };
            g.DrawString(title, titleFont, textBrush, titleRectangle.ToRectangleF(), stringFormat);
         }
      }
   }

   protected void drawAllSubTexts(Graphics graphics, UiActionType type, Rectangle clientRectangle)
   {
      var transparency = type is UiActionType.BusyText or UiActionType.ProgressDefinite or UiActionType.MuteProgress ? SubTextTransparency.Half
         : SubTextTransparency.None;

      var foreColor = new Lazy<Color>(getForeColor);
      var backColor = new Lazy<Color>(getBackColor);

      var _legend = legends.Peek();
      if (_legend is (true, var legend))
      {
         legend.Transparency = transparency;
         legend.Draw(graphics, foreColor.Value, backColor.Value);
      }

      if (Working && _working is (true, var working))
      {
         if (workingAlpha > 0)
         {
            working.Alpha = workingAlpha;
            working.Transparency = SubTextTransparency.Custom;
            workingAlpha -= 8;
         }

         working.SetLocation(clientRectangle);
         working.Draw(graphics, foreColor.Value, backColor.Value);
      }

      foreach (var subText in subTexts.Values)
      {
         subText.Transparency = transparency;
         subText.SetLocation(clientRectangle);
         subText.Draw(graphics, foreColor.Value, backColor.Value);
         subText.AdjustLeftSubText();
         subText.AdjustRightSubText();
      }
   }

   public void RelocateSubTexts()
   {
      var clientRectangle = glyphAdjustedClientRectangle();

      var _legend = legends.Peek();
      if (_legend is (true, var legend))
      {
         legend.SetLocation(clientRectangle);
      }

      if (Working && _working is (true, var working))
      {
         working.SetLocation(clientRectangle);
      }

      foreach (var subText in subTexts.Values)
      {
         subText.SetLocation(clientRectangle);
         subText.AdjustLeftSubText();
         subText.AdjustRightSubText();
      }
   }

   protected void drawClickGlyph(PaintEventArgs e, Rectangle clientRectangle, Color color)
   {
      if (ClickGlyph)
      {
         using var pen = new Pen(color, 4);
         if (Arrow)
         {
            var arrowSection = ClientRectangle.Width * START_AMOUNT;
            var arrowPoints = new PointF[]
            {
               new(arrowSection, 4),
               new(ClientRectangle.Width - 4, ClientRectangle.Height / 2.0f),
               new(arrowSection, ClientRectangle.Height - 4)
            };
            using var path = new GraphicsPath();
            path.AddLines(arrowPoints);

            e.Graphics.DrawPath(pen, path);
         }
         else
         {
            e.Graphics.DrawLine(pen, clientRectangle.Right - 4, 4, clientRectangle.Right - 4, clientRectangle.Bottom - 4);
            if (ChooserGlyph)
            {
               using var thinPen = new Pen(color, 1);
               var x1 = clientRectangle.Right - 8;
               var x2 = clientRectangle.Right - 16;
               var bottom = clientRectangle.Bottom - 4;
               for (var y = 4; y <= bottom; y += 2)
               {
                  e.Graphics.DrawLine(thinPen, x1, y, x2, y);
               }
            }
         }
      }
   }

   protected override void OnPaintBackground(PaintEventArgs pevent)
   {
      if (!Enabled && !_symbolWriter)
      {
         DisabledWriter.OnPaintBackground(pevent.Graphics, ClientRectangle);
         return;
      }

      base.OnPaintBackground(pevent);

      activateProcessor(pevent.Graphics);

      var clientRectangle = getClientRectangle();

      switch (type)
      {
         case UiActionType.Tape:
         {
            using var brush = new HatchBrush(HatchStyle.BackwardDiagonal, Color.Black, Color.Gold);
            fillRectangle(pevent.Graphics, brush, clientRectangle);
            break;
         }
         case UiActionType.ProgressIndefinite or UiActionType.Busy:
         {
            using var brush = new SolidBrush(Color.DarkSlateGray);
            fillRectangle(pevent.Graphics, brush, clientRectangle);
            break;
         }
         case UiActionType.ProgressDefinite when _progressDefiniteProcessor is (true, var progressDefiniteProcessor):
         {
            progressDefiniteProcessor.OnPaintBackground(pevent.Graphics);
            var textRectangle = progressDefiniteProcessor.TextRectangle;

            using var coralBrush = new SolidBrush(Color.Coral);
            fillRectangle(pevent.Graphics, coralBrush, textRectangle);
            var width = textRectangle.Width;
            var percentWidth = getPercentage(width);
            var location = textRectangle.Location;
            var size = new Size(percentWidth, textRectangle.Height);
            var rectangle = new Rectangle(location, size);
            using var cornflowerBlueBrush = new SolidBrush(Color.CornflowerBlue);
            fillRectangle(pevent.Graphics, cornflowerBlueBrush, rectangle);

            break;
         }
         case UiActionType.ProgressMini when _progressMiniProcessor is (true, var progressMiniProcessor):
         {
            progressMiniProcessor.OnPaintBackground(pevent.Graphics, value, maximum);
            break;
         }
         case UiActionType.MuteProgress:
         {
            using var coralBrush = new SolidBrush(Color.Coral);
            fillRectangle(pevent.Graphics, coralBrush, clientRectangle);
            var width = clientRectangle.Width;
            var percentWidth = getPercentage(width);
            var location = clientRectangle.Location;
            var size = new Size(percentWidth, clientRectangle.Height);
            var rectangle = new Rectangle(location, size);
            using var cornflowerBlueBrush = new SolidBrush(Color.CornflowerBlue);
            fillRectangle(pevent.Graphics, cornflowerBlueBrush, rectangle);

            break;
         }
         case UiActionType.Unselected:
         {
            using var brush = new SolidBrush(Color.White);
            fillRectangle(pevent.Graphics, brush, clientRectangle);

            using var pen = new Pen(Color.DarkGray, 10);
            drawRectangle(pevent.Graphics, pen, clientRectangle);
            break;
         }
         case UiActionType.Selected:
         {
            using var brush = new SolidBrush(Color.White);
            fillRectangle(pevent.Graphics, brush, clientRectangle);

            using var pen = new Pen(Color.Black, 10);
            drawRectangle(pevent.Graphics, pen, clientRectangle);
            break;
         }
         case UiActionType.BusyText when _busyTextProcessor is (true, var busyTextProcessor):
         {
            using var brush = new SolidBrush(Color.Teal);
            fillRectangle(pevent.Graphics, brush, clientRectangle);

            busyTextProcessor.OnPaint(pevent);

            break;
         }
         case UiActionType.ControlLabel:
         {
            using var brush = new SolidBrush(Color.CadetBlue);
            fillRectangle(pevent.Graphics, brush, clientRectangle);
            break;
         }
         case UiActionType.Http:
         {
            var httpWriter = new HttpWriter(text, clientRectangle, getFont());
            httpWriter.OnPaintBackground(pevent.Graphics, isUrlGood, mouseInside);
            break;
         }
         case UiActionType.Console:
            scroller.Value.OnPaintBackground(pevent.Graphics);
            break;
         case UiActionType.Display:
         {
            using var brush = new SolidBrush(BackColor);
            fillRectangle(pevent.Graphics, brush, clientRectangle);
            break;
         }
         case UiActionType.Symbol when _symbolWriter is (true, var symbolWriter):
            symbolWriter.OnPaintBackground(pevent.Graphics, clientRectangle, Enabled);
            break;
         case UiActionType.Divider:
            break;
         default:
         {
            var backColor = getBackColor();
            using var brush = new SolidBrush(backColor);
            fillRectangle(pevent.Graphics, brush, clientRectangle);
            break;
         }
      }

      if (isDirty && type is not UiActionType.Divider)
      {
         var backColor = getBackColor();
         var foreColor = ControlPaint.Light(backColor);
         using var brush = new HatchBrush(HatchStyle.DiagonalCross, foreColor, backColor);
         using var pen = new Pen(brush, 14);
         var leftSide = clientRectangle.Location;
         var rightSide = clientRectangle.NorthEast();
         pevent.Graphics.DrawLine(pen, leftSide, rightSide);
      }

      if (Is3D)
      {
         using var darkGrayPen = new Pen(Color.DarkGray, 1);
         using var lightPen = new Pen(Color.White, 1);

         var left = ClientRectangle.Left;
         var top = ClientRectangle.Top;
         var width = ClientRectangle.Width - 1;
         var height = ClientRectangle.Height - 1;

         pevent.Graphics.DrawLine(darkGrayPen, new Point(left, top), new Point(width, top));
         pevent.Graphics.DrawLine(darkGrayPen, new Point(left, top), new Point(left, height));
         pevent.Graphics.DrawLine(lightPen, new Point(left, height), new Point(width, height));
         pevent.Graphics.DrawLine(lightPen, new Point(width, top), new Point(width, height));
      }

      if (_image is (true, var image))
      {
         if (StretchImage)
         {
            pevent.Graphics.DrawImage(image, clientRectangle with { X = 0, Y = 0 });
         }
         else
         {
            var x = new Lazy<int>(() => centerHorizontal(image));
            var y = new Lazy<int>(() => centerVertical(image));
            var right = new Lazy<int>(() => rightHorizontal(image));
            var bottom = new Lazy<int>(() => bottomVertical(image));
            var location = CardinalAlignment switch
            {
               CardinalAlignment.Center => new Point(x.Value, y.Value),
               CardinalAlignment.North => new Point(x.Value, 2),
               CardinalAlignment.NorthEast => new Point(right.Value, 2),
               CardinalAlignment.East => new Point(right.Value, y.Value),
               CardinalAlignment.SouthEast => new Point(right.Value, bottom.Value),
               CardinalAlignment.South => new Point(x.Value, bottom.Value),
               CardinalAlignment.SouthWest => new Point(2, bottom.Value),
               CardinalAlignment.West => new Point(2, y.Value),
               CardinalAlignment.NorthWest => new Point(2, 2),
               _ => new Point(2, 2)
            };
            pevent.Graphics.DrawImage(image, location);
         }
      }

      PaintingBackground?.Invoke(this, pevent);

      return;

      int bottomVertical(Image image)
      {
         var y = clientRectangle.Height - image.Height;
         return y < 0 ? 2 : y;
      }

      int centerVertical(Image image)
      {
         var y = (clientRectangle.Height - image.Height) / 2;
         return y < 0 ? 2 : y;
      }

      int rightHorizontal(Image image)
      {
         var x = clientRectangle.Width - image.Width;
         return x < 0 ? 2 : x;
      }

      int centerHorizontal(Image image)
      {
         var x = (clientRectangle.Width - image.Width) / 2;
         return x < 0 ? 2 : x;
      }
   }

   protected static bool isMouseDown() => (MouseButtons & MouseButtons.Left) != 0;

   protected override void OnMouseEnter(EventArgs e)
   {
      base.OnMouseEnter(e);

      if (!mouseInside)
      {
         mouseInside = true;
         refresh();
      }
   }

   protected override void OnMouseLeave(EventArgs e)
   {
      base.OnMouseLeave(e);

      if (mouseInside)
      {
         mouseInside = false;
         refresh();
      }
   }

   protected override void OnMouseDown(MouseEventArgs e)
   {
      base.OnMouseDown(e);

      if (!mouseDown)
      {
         mouseDown = true;
         refresh();
      }
   }

   protected override void OnMouseUp(MouseEventArgs e)
   {
      base.OnMouseUp(e);

      if (mouseDown)
      {
         mouseDown = false;
         refresh();
      }
   }

   protected int getPercentage() => _percentage | (() => (int)((float)value / maximum * 100));

   protected int getPercentage(int width)
   {
      return _percentage.Map(p => (int)(width * (p / 100.0))) | (() => (int)((float)value / maximum * width));
   }

   public int Index(bool increment) => increment ? index++ : index;

   public void Busy(bool enabled)
   {
      if (enabled)
      {
         Text = "";
         type = UiActionType.Busy;
      }

      if (TaskBarProgress && !_taskBarProgress)
      {
         _taskBarProgress = getTaskBarProgress();
      }

      this.Do(() => timerPaint.Enabled = enabled);
   }

   public void Button(string text)
   {
      Text = text;
      type = UiActionType.Button;

      Refresh();
   }

   public void DefaultButton(string text)
   {
      ButtonType = UiActionButtonType.Default;
      Button(text);
   }

   public void CancelButton(string text)
   {
      ButtonType = UiActionButtonType.Cancel;
      Button(text);
   }

   public void StartAutomatic()
   {
      Text = "";
      type = UiActionType.Automatic;

      this.Do(() => timerPaint.Enabled = true);
   }

   public void StopAutomatic()
   {
      this.Do(() => timerPaint.Enabled = false);
   }

   public bool IsAutomaticRunning => timerPaint.Enabled;

   protected override void OnEnabledChanged(EventArgs e)
   {
      base.OnEnabledChanged(e);

      if (Enabled)
      {
         if (_lastType)
         {
            ShowMessage(text, _lastType);
            _lastType = nil;
         }
         else
         {
            ShowMessage(text, UiActionType.Uninitialized);
         }

         if (_lastEnabled is (true, true))
         {
            timerPaint.Enabled = true;
            _lastEnabled = nil;
         }

         _foreColor = _lastForeColor;
         _backColor = _lastBackColor;
         _style = _lastStyle;
      }
      else
      {
         _lastType = type;
         if (!_symbolWriter)
         {
            type = UiActionType.Disabled;
         }

         _lastEnabled = timerPaint.Enabled;
         timerPaint.Enabled = false;

         _lastForeColor = _foreColor;
         _foreColor = nil;

         _lastBackColor = _backColor;
         _backColor = nil;

         _lastStyle = _style;
         _style = nil;

         refresh();
      }
   }

   protected virtual void drawRectangle(Graphics graphics, Pen pen, Rectangle rectangle) => graphics.DrawRectangle(pen, rectangle);

   protected virtual void drawLine(Graphics graphics, Color color, ((int x, int y), (int width, int height)) coordinates, float penWidth = 4,
      bool dashed = true)
   {
      var ((x, y), (width, height)) = coordinates;
      var x1 = x + width;
      var y1 = y + height;

      graphics.HighQuality();

      using var pen = new Pen(color, penWidth);
      if (dashed)
      {
         pen.DashPattern = [3.0f, 1.0f];
      }

      graphics.DrawLine(pen, x, y, x1, y1);
   }

   protected bool fillArrowRectangle(Graphics graphics, Brush brush, Rectangle rectangle)
   {
      if (Arrow)
      {
         graphics.HighQuality();
         var arrowSection = rectangle.Width * START_AMOUNT;
         var arrowPoints = new PointF[]
         {
            new(0, 0),
            new(arrowSection, 0),
            new(rectangle.Width, rectangle.Height / 2.0f),
            new(arrowSection, Height),
            new(0, rectangle.Height),
            new(rectangle.Width - arrowSection, rectangle.Height / 2.0f),
            new(0, 0)
         };

         using var path = new GraphicsPath();
         path.AddLines(arrowPoints);
         path.CloseFigure();

         graphics.FillPath(brush, path);

         return true;
      }
      else
      {
         return false;
      }
   }

   protected virtual void fillRectangle(Graphics graphics, Brush brush, Rectangle rectangle)
   {
      if (!fillArrowRectangle(graphics, brush, rectangle))
      {
         graphics.FillRectangle(brush, rectangle);
      }
   }

   protected void setFloor(int amount)
   {
      if (_floor is (true, var floor))
      {
         _floor = floor.MaxOf(amount);
      }
      else
      {
         _floor = amount;
      }
   }

   protected void setCeiling(int amount)
   {
      if (_ceiling is (true, var ceiling))
      {
         _ceiling = ceiling.MinOf(amount);
      }
      else
      {
         _ceiling = amount;
      }
   }

   protected void setFloorAndCeiling(int x, int y, Size size, bool includeFloor, bool includeCeiling)
   {
      setFloorAndCeiling(new Rectangle(new Point(x, y), size), includeFloor, includeCeiling);
   }

   protected void setFloorAndCeiling(int x, int y, int width, int height, bool includeFloor, bool includeCeiling)
   {
      setFloorAndCeiling(x, y, new Size(width, height), includeFloor, includeCeiling);
   }

   protected void setFloorAndCeiling(SubText subText)
   {
      setFloorAndCeiling(subText.X, subText.Y, subText.TextSize(nil).measuredSize, subText.IncludeFloor, subText.IncludeCeiling);
   }

   protected void setFloorAndCeiling(Rectangle rectangle, bool includeFloor, bool includeCeiling)
   {
      var halfway = ClientRectangle.Width / 2;
      var floor = rectangle.Left + rectangle.Width;
      var isLeft = floor < halfway;
      if (isLeft)
      {
         if (includeFloor)
         {
            setFloor(floor);
         }
      }
      else if (includeCeiling)
      {
         setCeiling(rectangle.Left);
      }
   }

   protected void determineFloorAndCeiling()
   {
      _floor = nil;
      _ceiling = nil;

      if (checkStyle is not CheckStyle.None)
      {
         setFloorAndCeiling(2, 2, 12, 12, true, true);
      }

      foreach (var subText in subTexts.Values)
      {
         setFloorAndCeiling(subText);
      }

      if (Stopwatch)
      {
         var elapsed = stopwatch.Value.Elapsed.ToString(@"mm\:ss");
         using var stopwatchFont = new Font("Consolas", 10);
         var size = TextRenderer.MeasureText(elapsed, stopwatchFont, Size.Empty);
         var location = new Point(ClientRectangle.Width - size.Width - 8, 4);
         var rectangle = new Rectangle(location, size);
         setFloorAndCeiling(rectangle, true, true);
      }

      if (type is UiActionType.BusyText)
      {
         setFloorAndCeiling(new Rectangle(0, 0, ClientRectangle.Height, ClientRectangle.Height), true, true);
      }

      if (type is UiActionType.ProgressDefinite)
      {
         var size = TextRenderer.MeasureText("100%", Font, Size.Empty);
         size = size with { Height = ClientRectangle.Height };
         setFloorAndCeiling(new Rectangle(ClientRectangle.Location, size), true, true);
      }
   }

   public SubText SubText(SubText subText)
   {
      subTexts[subText.Id] = subText;
      determineFloorAndCeiling();

      return subText;
   }

   public SubText SubText(string text, int x, int y, bool clickable = false)
   {
      var subText = clickable ? new ClickableSubText(this, text, x, y, ClientSize, ClickGlyph, ChooserGlyph)
         : new SubText(this, text, x, y, ClientSize, ClickGlyph, ChooserGlyph);
      return SubText(subText);
   }

   public SubText SubText(string text, bool clickable = false) => SubText(text, 0, 0, clickable);

   public ClickableSubText ClickableSubText(string text, int x, int y)
   {
      return (ClickableSubText)SubText(new ClickableSubText(this, text, x, y, ClientSize, ClickGlyph, ChooserGlyph));
   }

   public ClickableSubText ClickableSubText(string text) => ClickableSubText(text, 0, 0);

   public void RemoveSubText(Guid id)
   {
      subTexts.Remove(id);
      determineFloorAndCeiling();
      refresh();
   }

   public void RemoveSubText(SubText subText) => RemoveSubText(subText.Id);

   public void RemoveSubText(Maybe<SubText> _subText)
   {
      if (_subText is (true, var subText))
      {
         RemoveSubText(subText);
      }
   }

   public void ClearSubTexts()
   {
      subTexts.Clear();
      determineFloorAndCeiling();
      refresh();
   }

   public void RunAsync()
   {
      var args = new ArgumentsArgs();
      Arguments?.Invoke(this, args);
      var _arguments = args.Arguments;
      if (_arguments is (true, var arguments))
      {
         RunWorkerAsync(arguments);
      }
      else
      {
         RunWorkerAsync();
      }
   }

   public void RunWorkerAsync()
   {
      var args = new InitializeArgs();
      this.Do(() => Initialize?.Invoke(this, args));
      if (!args.Cancel && !backgroundWorker.Value.IsBusy)
      {
         var _argument = args.Argument;
         if (_argument is (true, var argument))
         {
            backgroundWorker.Value.RunWorkerAsync(argument);
         }
         else
         {
            backgroundWorker.Value.RunWorkerAsync();
         }
      }
   }

   public void RunWorkerAsync(object argument)
   {
      var args = new InitializeArgs { Argument = argument };
      this.Do(() => Initialize?.Invoke(this, args));
      if (!args.Cancel && !backgroundWorker.Value.IsBusy)
      {
         var _argument = args.Argument;
         if (_argument is (true, var argumentValue))
         {
            backgroundWorker.Value.RunWorkerAsync(argumentValue);
         }
         else
         {
            backgroundWorker.Value.RunWorkerAsync();
         }
      }
   }

   public bool IsBusy => backgroundWorker.Value.IsBusy;

   public bool WorkReportsProgress
   {
      get => backgroundWorker.Value.WorkerReportsProgress;
      set => backgroundWorker.Value.WorkerReportsProgress = value;
   }

   public void ReportProgress(int percentProgress, object userState) => backgroundWorker.Value.ReportProgress(percentProgress, userState);

   public void ReportProgress(int percentProgress) => backgroundWorker.Value.ReportProgress(percentProgress);

   public void CancelAsync() => backgroundWorker.Value.CancelAsync();

   public bool CancellationPending => backgroundWorker.Value.CancellationPending;

   public void StartTimer() => timer.Enabled = true;

   public void StartTimer(TimeSpan interval, bool oneTime = false)
   {
      oneTimeTimer = oneTime;
      timer.Interval = (int)interval.TotalMilliseconds;
      timer.Enabled = true;
   }

   public void StopTimer()
   {
      timer.Enabled = false;
      oneTimeTimer = false;
   }

   public void NotifyDefault(bool value) => ButtonType = value ? UiActionButtonType.Default : UiActionButtonType.Cancel;

   public void PerformClick() => OnClick(EventArgs.Empty);

   public DialogResult DialogResult { get; set; }

   public bool TimerEnabled => timer.Enabled;

   protected (int, int) legendLocation() => CheckStyle != CheckStyle.None ? (20, 2) : (2, 2);

   public SubText Legend(string text, bool invert = true)
   {
      var (x, y) = legendLocation();
      var legend = new SubText(this, text, x, y, ClientSize, true, ChooserGlyph)
         .Set
         .FontSize(8)
         .Outline()
         .Invert(invert)
         .SubText;
      legends.Push(legend);

      determineFloorAndCeiling();

      return legend;
   }

   public SubText Legend(string text, int x, int y, bool invert = true)
   {
      var legend = new SubText(this, text, x, y, ClientSize, true, ChooserGlyph)
         .Set
         .FontSize(8)
         .Outline()
         .Invert(invert)
         .SubText;
      legends.Push(legend);

      determineFloorAndCeiling();

      return legend;
   }

   public async Task LegendAsync(string text, TimeSpan delay, bool invert = true)
   {
      Legend(text, invert);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task LegendAsync(string text, bool invert = true) => await LegendAsync(text, 2.Seconds(), invert);

   public async Task LegendAsync(string text, int x, int y, TimeSpan delay, bool invert = true)
   {
      Legend(text, x, y, invert);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task LegendAsync(string text, int x, int y, bool invert = true)
   {
      await LegendAsync(text, x, y, 2.Seconds(), invert);
   }

   public void LegendTemp(string text, TimeSpan delay, bool invert = true)
   {
      this.Do(() => Task.Run(() => LegendAsync(text, delay, invert)));
   }

   public void LegendTemp(string text, bool invert = true) => LegendTemp(text, 2.Seconds(), invert);

   public void LegendTemp(string text, int x, int y, TimeSpan delay, bool invert = true)
   {
      this.Do(() => Task.Run(() => LegendAsync(text, x, y, delay, invert)));
   }

   public void LegendTemp(string text, int x, int y, bool invert = true) => LegendTemp(text, x, y, 2.Seconds(), invert);

   public SubText SuccessLegend(string text)
   {
      return Legend(text, false).Set.ForeColor(Color.White).BackColor(Color.Green).SubText;
   }

   public SubText SuccessLegend(string text, int x, int y)
   {
      return Legend(text, x, y, false).Set.ForeColor(Color.White).BackColor(Color.Green).SubText;
   }

   public async Task SuccessLegendAsync(string text, TimeSpan delay)
   {
      SuccessLegend(text);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task SuccessLegendAsync(string text) => await SuccessLegendAsync(text, 2.Seconds());

   public async Task SuccessLegendAsync(string text, int x, int y, TimeSpan delay)
   {
      SuccessLegend(text, x, y);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task SuccessLegendAsync(string text, int x, int y) => await SuccessLegendAsync(text, x, y, 2.Seconds());

   public void SuccessLegendTemp(string text, TimeSpan delay) => this.Do(() => Task.Run(() => SuccessLegendAsync(text, delay)));

   public void SuccessLegendTemp(string text) => SuccessLegendTemp(text, 2.Seconds());

   public void SuccessLegendTemp(string text, int x, int y, TimeSpan delay)
   {
      this.Do(() => Task.Run(() => SuccessLegendAsync(text, x, y, delay)));
   }

   public void SuccessLegendTemp(string text, int x, int y) => SuccessLegendTemp(text, x, y, 2.Seconds());

   public SubText FailureLegend(string text)
   {
      return Legend(text, false).Set.ForeColor(Color.Black).BackColor(Color.Gold).SubText;
   }

   public SubText FailureLegend(string text, int x, int y)
   {
      return Legend(text, x, y, false).Set.ForeColor(Color.Black).BackColor(Color.Gold).SubText;
   }

   public async Task FailureLegendAsync(string text, TimeSpan delay)
   {
      FailureLegend(text);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task FailureLegendAsync(string text) => await FailureLegendAsync(text, 2.Seconds());

   public async Task FailureLegendAsync(string text, int x, int y, TimeSpan delay)
   {
      FailureLegend(text, x, y);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task FailureLegendAsync(string text, int x, int y) => await FailureLegendAsync(text, x, y, 2.Seconds());

   public void FailureLegendTemp(string text, TimeSpan delay) => this.Do(() => Task.Run(() => FailureLegendAsync(text, delay)));

   public void FailureLegendTemp(string text) => FailureLegendTemp(text, 2.Seconds());

   public void FailureLegendTemp(string text, int x, int y, TimeSpan delay)
   {
      this.Do(() => Task.Run(() => FailureLegendAsync(text, x, y, delay)));
   }

   public void FailureLegendTemp(string text, int x, int y) => FailureLegendTemp(text, x, y, 2.Seconds());

   public SubText ExceptionLegend(Exception exception)
   {
      return Legend(exception.Message, false).Set.ForeColor(Color.White).BackColor(Color.Red).SubText;
   }

   public SubText ExceptionLegend(Exception exception, int x, int y)
   {
      return Legend(exception.Message, x, y, false).Set.ForeColor(Color.White).BackColor(Color.Red).SubText;
   }

   public async Task ExceptionLegendSync(Exception exception, TimeSpan delay)
   {
      ExceptionLegend(exception);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task ExceptionLegendSync(Exception exception) => await ExceptionLegendSync(exception, 2.Seconds());

   public async Task ExceptionLegendSync(Exception exception, int x, int y, TimeSpan delay)
   {
      ExceptionLegend(exception, x, y);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task ExceptionLegendSync(Exception exception, int x, int y) => await ExceptionLegendSync(exception, x, y, 2.Seconds());

   public void ExceptionLegendTemp(Exception exception, TimeSpan delay) => this.Do(() => Task.Run(() => ExceptionLegendSync(exception, delay)));

   public void ExceptionLegendTemp(Exception exception) => ExceptionLegendTemp(exception, 2.Seconds());

   public void ExceptionLegendTemp(Exception exception, int x, int y, TimeSpan delay)
   {
      this.Do(() => Task.Run(() => ExceptionLegendSync(exception, x, y, delay)));
   }

   public void ExceptionLegendTemp(Exception exception, int x, int y) => ExceptionLegendTemp(exception, x, y, 2.Seconds());

   public SubText ResultLegend(Result<string> _result)
   {
      if (_result is (true, var result))
      {
         return SuccessLegend(result);
      }
      else
      {
         return ExceptionLegend(_result.Exception);
      }
   }

   public SubText ResultLegend(Result<string> _result, int x, int y)
   {
      if (_result is (true, var result))
      {
         return SuccessLegend(result, x, y);
      }
      else
      {
         return ExceptionLegend(_result.Exception, x, y);
      }
   }

   public async Task ResultLegendAsync(Result<string> _result, TimeSpan delay)
   {
      ResultLegend(_result);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task ResultLegendAsync(Result<string> _result) => await ResultLegendAsync(_result, 2.Seconds());

   public async Task ResultLegendAsync(Result<string> _result, int x, int y, TimeSpan delay)
   {
      ResultLegend(_result, x, y);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task ResultLegendAsync(Result<string> _result, int x, int y) => await ResultLegendAsync(_result, x, y, 2.Seconds());

   public void ResultLegendTemp(Result<string> _result, TimeSpan delay) => this.Do(() => Task.Run(() => ResultLegendAsync(_result, delay)));

   public void ResultLegendTemp(Result<string> _result) => ResultLegendTemp(_result, 2.Seconds());

   public void ResultLegendTemp(Result<string> _result, int x, int y, TimeSpan delay)
   {
      this.Do(() => Task.Run(() => ResultLegendAsync(_result, x, y, delay)));
   }

   public void ResultLegendTemp(Result<string> _result, int x, int y) => ResultLegendTemp(_result, x, y, 2.Seconds());

   public SubText ResultLegend((string, UiActionType) result)
   {
      var (message, uiActionType) = result;
      return uiActionType switch
      {
         UiActionType.Success => SuccessLegend(message),
         UiActionType.Failure => FailureLegend(message),
         UiActionType.Exception => ExceptionLegend(fail(message)),
         _ => Legend(message)
      };
   }

   public SubText ResultLegend((string, UiActionType) result, int x, int y)
   {
      var (message, uiActionType) = result;
      return uiActionType switch
      {
         UiActionType.Success => SuccessLegend(message, x, y),
         UiActionType.Failure => FailureLegend(message, x, y),
         UiActionType.Exception => ExceptionLegend(fail(message), x, y),
         _ => Legend(message, x, y)
      };
   }

   public async Task ResultLegendAsync((string, UiActionType) result, TimeSpan delay)
   {
      ResultLegend(result);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task ResultLegendAsync((string, UiActionType) result) => await ResultLegendAsync(result, 2.Seconds());

   public async Task ResultLegendAsync((string, UiActionType) result, int x, int y, TimeSpan delay)
   {
      ResultLegend(result, x, y);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task ResultLegendAsync((string, UiActionType) result, int x, int y) => await ResultLegendAsync(result, x, y, 2.Seconds());

   public void ResultLegendTemp((string, UiActionType) result, TimeSpan delay) => this.Do(() => Task.Run(() => ResultLegendAsync(result, delay)));

   public void ResultLegendTemp((string, UiActionType) result) => ResultLegendTemp(result, 2.Seconds());

   public void ResultLegendTemp((string, UiActionType) result, int x, int y, TimeSpan delay)
   {
      this.Do(() => Task.Run(() => ResultLegendAsync(result, x, y, delay)));
   }

   public void ResultLegendTemp((string, UiActionType) result, int x, int y) => ResultLegendTemp(result, x, y, 2.Seconds());

   public SubText Notify(string text) => Legend(text, false).Set.ForeColor(Color.Black).BackColor(Color.White).SubText;

   public SubText Notify(string text, int x, int y) => Legend(text, x, y, false).Set.ForeColor(Color.Black).BackColor(Color.White).SubText;

   public async Task NotifyAsync(string text, TimeSpan delay)
   {
      Notify(text);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task NotifyAsync(string text) => await NotifyAsync(text, 2.Seconds());

   public async Task NotifyAsync(string text, int x, int y, TimeSpan delay)
   {
      Notify(text, x, y);
      refresh();

      await Task.Delay(delay).ContinueWith(_ => Legend());
   }

   public async Task NotifyAsync(string text, int x, int y) => await NotifyAsync(text, x, y, 2.Seconds());

   public void NotifyTemp(string text, TimeSpan delay) => this.Do(() => Task.Run(() => NotifyAsync(text, delay)));

   public void NotifyTemp(string text) => NotifyTemp(text, 2.Seconds());

   public void NotifyTemp(string text, int x, int y, TimeSpan delay) => this.Do(() => Task.Run(() => NotifyAsync(text, x, y, delay)));

   public void NotifyTemp(string text, int x, int y) => NotifyTemp(text, x, y, 2.Seconds());

   public void Legend()
   {
      legends.Pop();
      refresh();
   }

   public void ClearAllLegends()
   {
      legends.Clear();
      refresh();
   }

   public Maybe<SubText> CurrentLegend => legends.Peek();

   public Maybe<string> Working
   {
      get => _workingText;
      set
      {
         _workingText = value;
         this.Do(() => workingTimer.Enabled = value);
         _working = nil;
      }
   }

   public CardinalAlignment WorkingAlignment
   {
      get => workingAlignment;
      set => workingAlignment = value;
   }

   public void Validate(string text)
   {
      var args = new ValidatedArgs(text);
      ValidateText?.Invoke(this, args);
      ShowMessage(text, args.Type);
   }

   public bool ShowFocus { get; set; }

   public Maybe<string> EmptyTextTitle { get; set; } = nil;

   public async Task<Completion<TResult>> ExecuteAsync<TArgument, TResult>(TArgument argument, Func<TArgument, Completion<TResult>> func,
      Action<TResult> postAction) where TResult : notnull
   {
      var _result = await ExecuteAsync(argument, func);
      if (_result is (true, var result))
      {
         postAction(result);
      }

      return _result;
   }

   public async Task<Completion<TResult>> ExecuteAsync<TArgument, TResult>(TArgument argument, Func<TArgument, Completion<TResult>> func)
      where TResult : notnull
   {
      return await Task.Run(() => func(argument));
   }

   public async Task<Completion<Unit>> ExecuteAsync<TArgument>(TArgument argument, Func<TArgument, Completion<Unit>> func, Action postAction)
   {
      var _result = await ExecuteAsync(argument, func);
      if (_result)
      {
         postAction();
      }

      return _result;
   }

   public async Task<Completion<Unit>> ExecuteAsync<TArgument>(TArgument argument, Func<TArgument, Completion<Unit>> func)
   {
      return await Task.Run(() => func(argument));
   }

   public async Task<Completion<TResult>> ExecuteAsync<TResult>(Func<Completion<TResult>> func, Action<TResult> postAction) where TResult : notnull
   {
      var _result = await ExecuteAsync(func);
      if (_result is (true, var result))
      {
         postAction(result);
      }

      return _result;
   }

   public async Task<Completion<TResult>> ExecuteAsync<TResult>(Func<Completion<TResult>> func) where TResult : notnull => await Task.Run(func);

   public async Task<Completion<Unit>> ExecuteAsync(Func<Completion<Unit>> func, Action postAction)
   {
      var _result = await ExecuteAsync(func);
      if (_result)
      {
         postAction();
      }

      return _result;
   }

   public async Task<Completion<Unit>> ExecuteAsync(Func<Completion<Unit>> func) => await Task.Run(func);

   public bool IsDirty
   {
      get => isDirty;
      set
      {
         isDirty = value;
         refresh();
      }
   }

   public ChooserSet Choose(string title, int width)
   {
      var chooser = new Chooser(title, this, width);
      return new ChooserSet(chooser);
   }

   public ChooserSet Choose(string title)
   {
      var chooser = new Chooser(title, this, nil);
      return new ChooserSet(chooser);
   }

   public void OnAppearanceOverride(AppearanceOverrideArgs e) => AppearanceOverride?.Invoke(this, e);

   public void OnChosenItemChecked(ChosenArgs e) => ChosenItemChecked?.Invoke(this, e);

   public void OnChosenItemSelected(ChosenArgs e) => ChosenItemSelected?.Invoke(this, e);

   public void OnChooserOpened() => ChooserOpened?.Invoke(this, EventArgs.Empty);

   public void OnChooserClosed() => ChooserClosed?.Invoke(this, EventArgs.Empty);

   public bool Arrow { get; set; }

   public void ControlLabel(string text) => ShowMessage(text, UiActionType.ControlLabel);

   public void Http(string url)
   {
      isUrlGood = HttpWriter.IsGoodUrl(url);
      ShowMessage(url, UiActionType.Http);
   }

   protected void openUrl(object? sender, EventArgs e)
   {
      if (text.IsNotEmpty())
      {
         using var process = new Process();
         process.StartInfo.UseShellExecute = true;
         process.StartInfo.FileName = text;
         process.Start();
      }
   }

   public void SizeToText()
   {
      var size = TextRenderer.MeasureText(text, font);
      Width = size.Width + 40;
   }

   public void WriteLine(object obj)
   {
      Type = UiActionType.Console;
      scroller.Value.WriteLine(obj);

      MessageShown?.Invoke(this, new MessageShownArgs(text, type));

      refresh();
   }

   public void FloatingFailure(string message)
   {
      FloatingFailure(false);

      _failureToolTip = message;
      _failureSubText = SubText("failure").Set.GoToUpperLeft(0).Font("Consolas", 8).ForeColor(Color.Black).BackColor(Color.Gold).SubText;

      setToolTip();
   }

   public void FloatingFailure(bool set = true)
   {
      _failureToolTip = nil;
      if (_failureSubText is (true, var failureSubText))
      {
         subTexts.Remove(failureSubText.Id);
      }

      _failureSubText = nil;

      if (set)
      {
         setToolTip();
      }
   }

   public void FloatingNoStatus(string message)
   {
      FloatingNoStatus(false);

      _noStatusToolTip = message;
      _noStatusSubText = SubText("no status").Set.GoToUpperLeft(0).Font("Consolas", 8).ForeColor(Color.Black).BackColor(Color.White).SubText;
   }

   public void FloatingNoStatus(bool set = true)
   {
      _noStatusToolTip = nil;
      if (_noStatusSubText is (true, var noStatusSubText))
      {
         subTexts.Remove(noStatusSubText.Id);
      }

      _noStatusSubText = nil;

      if (set)
      {
         setToolTip();
      }
   }

   public Maybe<string> SuccessToolTip
   {
      get => _successToolTip;
      set => _successToolTip = value;
   }

   public Maybe<string> FailureToolTip
   {
      get => _failureToolTip;
      set => _failureToolTip = value;
   }

   public Maybe<string> NoStatusToolTip
   {
      get => _noStatusToolTip;
      set => _noStatusToolTip = value;
   }

   public void FloatingException(Exception exception)
   {
      FloatingException(false);

      _exceptionToolTip = exception.Message;
      _exceptionSubText = SubText("exception").Set.GoToUpperLeft(0).Font("Consolas", 8).ForeColor(Color.White).BackColor(Color.Red).SubText;

      setToolTip();
   }

   public void FloatingException(bool set = true)
   {
      _exceptionToolTip = nil;
      if (_exceptionSubText is (true, var exceptionSubText))
      {
         subTexts.Remove(exceptionSubText.Id);
      }

      _exceptionSubText = nil;

      if (set)
      {
         setToolTip();
      }
   }

   public bool FlipFlop { get; set; }

   public Maybe<string> ExceptionToolTip
   {
      get => _exceptionToolTip;
      set => _exceptionToolTip = value;
   }

   public bool HasFloatingFailureOrException => _failureToolTip || _exceptionToolTip;

   public void ClearFloating()
   {
      FloatingFailure(false);
      FloatingException(false);
      FloatingNoStatus(false);

      setToolTip();
   }

   public string ToolTipTitle
   {
      get => toolTip.ToolTipTitle;
      set
      {
         toolTip.ToolTipTitle = value;
         setToolTip();
      }
   }

   public bool ToolTipBox
   {
      get => toolTip.ToolTipBox;
      set
      {
         toolTip.ToolTipBox = value;
         setToolTip();
      }
   }

   public Maybe<SubText> ProgressSubText
   {
      get => _progressSubText;
      set
      {
         if (_progressSubText is (true, var progressSubText))
         {
            RemoveSubText(progressSubText);
         }

         _progressSubText = value;
      }
   }

   public bool IsFailureOrException => type is UiActionType.Failure or UiActionType.Exception;

   public bool IsABusyType
   {
      get
      {
         return type is UiActionType.Busy or UiActionType.BusyText or UiActionType.ProgressDefinite or UiActionType.ProgressIndefinite
            or UiActionType.MuteProgress;
      }
   }

   protected override void OnResize(EventArgs e)
   {
      base.OnResize(e);

      foreach (var (_, subText) in subTexts)
      {
         subText.ResetLock();
         var clientRectangle = getClientRectangle();
         subText.SetLocation(clientRectangle);
      }
   }

   public int RectangleCount
   {
      get => rectangles.Length;
      set
      {
         if (value > 0)
         {
            var clientWidth = ClientRectangle.Width;
            var clientHeight = ClientRectangle.Height;

            var paddingWidth = 2;
            var width = (clientWidth - (value + 1) * paddingWidth) / value;
            var top = paddingWidth;
            var height = clientHeight - 2 * paddingWidth;
            var fullWidth = paddingWidth + width;

            rectangles = [.. Enumerable.Range(0, value).Select(i => new Rectangle(i * fullWidth, top, width, height))];
         }
         else
         {
            rectangles = [];
         }
      }
   }

   public Rectangle[] Rectangles => rectangles;

   public void KeyMatch(Keys keys, string downMessage, string upMessage, TimeSpan elapsedTime)
   {
      _keyMatch = new KeyMatch(keys, this, downMessage, upMessage, elapsedTime);
   }

   public void KeyMatch(Keys keys, string downMessage, string upMessage) => KeyMatch(keys, downMessage, upMessage, 500.Milliseconds());

   public void KeyMatch(string downMessage, string upMessage, TimeSpan elapsedTime) => KeyMatch(Keys.Control, downMessage, upMessage, elapsedTime);

   public void KeyMatch(string downMessage, string upMessage) => KeyMatch(downMessage, upMessage, 500.Milliseconds());

   public bool IsKeyDown => _keyMatch.Map(km => km.IsDown) | false;

   public void Symbol(UiActionSymbol symbol, Color foreColor, Color backColor)
   {
      _symbolWriter = symbol switch
      {
         UiActionSymbol.Plus => new PlusSymbolWriter(foreColor, backColor),
         UiActionSymbol.Minus => new MinusSymbolWriter(foreColor, backColor),
         UiActionSymbol.Menu => new MenuSymbolWriter(foreColor, backColor),
         UiActionSymbol.X => new XSymbolWriter(foreColor, backColor),
         UiActionSymbol.O => new OSymbolWriter(foreColor, backColor),
         _ => nil
      };
      if (_symbolWriter)
      {
         type = UiActionType.Symbol;
         refresh();
      }
      else
      {
         Display($"{symbol}?", foreColor, backColor);
      }
   }

   public void Symbol(UiActionSymbol symbol, UiActionType type)
   {
      var foreColor = getForeColor(type);
      var backColor = getBackColor(type);
      Symbol(symbol, foreColor, backColor);
   }

   public UiActionButtonType ButtonType
   {
      get => buttonType;
      set
      {
         buttonType = value;
         Refresh();
      }
   }

   public void Alternate(params string[] alternates) => createAlternate(alternates);

   public void AlternateDeletable(params string[] alternates) => createDeletable(alternates);

   protected void createAlternate(string[] alternates)
   {
      if (alternates.Length < 1)
      {
         Failure("You should have at least one alternate");
         return;
      }

      FloatingException(false);
      Busy(false);
      _taskBarProgress = nil;

      type = UiActionType.Alternate;
      RectangleCount = alternates.Length;
      _alternateWriter = new AlternateWriter(this, alternates, AutoSizeText, _floor, _ceiling, UseEmojis);
      refresh();
   }

   protected void createDeletable(string[] alternates)
   {
      if (alternates.Length < 1)
      {
         Failure("You should have at least one alternate");
         return;
      }

      FloatingException(false);
      Busy(false);
      _taskBarProgress = nil;

      type = UiActionType.Alternate;
      RectangleCount = alternates.Length;
      _alternateWriter = new DeletableWriter(this, alternates, AutoSizeText, _floor, _ceiling, UseEmojis);
      refresh();
   }

   public void CheckBox(string message, bool initialValue)
   {
      FloatingException(false);
      Busy(false);
      _taskBarProgress = nil;

      type = UiActionType.CheckBox;
      setUpCheckBox(message, initialValue);
      refresh();
   }

   protected void setUpCheckBox(string message, bool initialValue)
   {
      RectangleCount = 1;
      var checkBoxWriter = new CheckBoxWriter(this, [message], AutoSizeText, _floor, _ceiling) { BoxChecked = initialValue };
      _alternateWriter = checkBoxWriter;
      Text = message;
   }

   public int SelectedIndex
   {
      get => _alternateWriter.Map(w => w.SelectedIndex) | -1;
      set
      {
         if (_alternateWriter is (true, var alternateWriter))
         {
            alternateWriter.SelectedIndex = value;
            if (ClickOnAlternate is not null)
            {
               var rectangleIndex = alternateWriter.SelectedIndex;
               var location = Rectangles[rectangleIndex].Location;
               var alternate = alternateWriter.Alternate;
               ClickOnAlternate.Invoke(this, new UiActionAlternateArgs(rectangleIndex, location, alternate, false));
               refresh();
            }
         }
      }
   }

   public string SelectedAlternate
   {
      get => _alternateWriter.Map(w => w.Alternate);
      set
      {
         if (_alternateWriter is (true, var alternateWriter))
         {
            var _index = alternateWriter.Alternates.Find(value);
            if (_index is (true, var selectedIndex))
            {
               alternateWriter.SelectedIndex = selectedIndex;
            }
         }
      }
   }

   public int DisabledIndex
   {
      get => _alternateWriter.Map(w => w.DisabledIndex) | -1;
      set
      {
         if (_alternateWriter is (true, var alternateWriter))
         {
            alternateWriter.DisabledIndex = value;
            refresh();
         }
      }
   }

   public bool BoxChecked
   {
      get => _alternateWriter.Map(w => ((CheckBoxWriter)w).BoxChecked) | false;
      set
      {
         if (_alternateWriter is (true, CheckBoxWriter checkBoxWriter))
         {
            checkBoxWriter.BoxChecked = value;
            Refresh();
         }
      }
   }

   public void SetForeColor(int index, Color color)
   {
      if (_alternateWriter is (true, var alternateWriter))
      {
         alternateWriter.SetForeColor(index, color);
      }
   }

   public Maybe<Color> GetForeColor(int index) => _alternateWriter.Map(w => w.GetForeColor(index));

   public void SetBackColor(int index, Color color)
   {
      if (_alternateWriter is (true, var alternateWriter))
      {
         alternateWriter.SetBackColor(index, color);
      }
   }

   public Maybe<Color> GetBackColor(int index) => _alternateWriter.Map(w => w.GetBackColor(index));

   public Color GetForeColor(UiActionType type) => getForeColor(type);

   public Color GetBackColor(UiActionType type) => getBackColor(type);

   public void SetColors(int index, UiActionType type)
   {
      SetForeColor(index, GetForeColor(type));
      SetBackColor(index, GetBackColor(type));
      refresh();
   }

   public Maybe<Color> GetAlternateForeColor(int index) => _alternateWriter.Map(w => w.GetAlternateForeColor(index));

   public Maybe<Color> GetAlternateBackColor(int index) => _alternateWriter.Map(w => w.GetAlternateBackColor(index));

   public void RemoveAlternate(int index)
   {
      if (_alternateWriter is (true, var alternateWriter))
      {
         var alternates = alternateWriter.Alternates;
         if (index < alternates.Length)
         {
            alternates = alternates.RemoveAt(index);
            if (alternateWriter is DeletableWriter)
            {
               createDeletable(alternates);
            }
            else
            {
               createAlternate(alternates);
            }
         }
      }
   }

   public Maybe<int> CurrentPositionIndex
   {
      get
      {
         var position = this.CursorPosition();
         return rectangles.Indexed().FirstOrNone(t => t.item.Contains(position)).Map(t => t.index);
      }
   }

   public string[] Alternates => _alternateWriter.Map(w => w.Alternates) | [];

   public Maybe<string> GetAlternate(int index) => _alternateWriter.Map(w => w.GetAlternate(index));

   public void SetLocation(SubText subText)
   {
      var clientRectangle = getClientRectangle();
      subText.SetLocation(clientRectangle);
   }

   public void SetLocation(Guid subTextId)
   {
      if (subTexts.Maybe[subTextId] is (true, var subText))
      {
         SetLocation(subText);
      }
   }

   public void SetLocation(SubText subText, int index)
   {
      if (index.Between(0).Until(rectangles.Length))
      {
         subText.SetLocation(rectangles[index]);
      }
   }

   public void SetLocation(Guid subTextId, int index)
   {
      if (subTexts.Maybe[subTextId] is (true, var subText))
      {
         SetLocation(subText, index);
      }
   }

   public void ResetLocks()
   {
      foreach (var subText in subTexts.Values)
      {
         subText.ResetLock();
      }

      Refresh();
   }

   public void Pulse()
   {
      workingAlpha = 255;
      Refresh();
   }

   public bool Required { get; set; }

   public void Divider(string message) => ShowMessage(message, UiActionType.Divider);

   protected Rectangle getDividerRectangle()
   {
      var rectangle = getClientRectangle();
      var dividerRectangle = rectangle with { Height = ClientSize.Height / 2 };
      return dividerRectangle with { Location = rectangle.West() };
   }

   protected Rectangle getDividerTextRectangle(Graphics g, Rectangle rectangleRectangle)
   {
      var flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
      var textSize = UiActionWriter.TextSize(g, text, font, flags, UseEmojis);

      var offset = 4;

      var left = 4 * offset;
      var top = rectangleRectangle.Height / 2 - textSize.Height / 2 - offset;
      var width = textSize.Width + 2 * offset;
      var height = textSize.Height + 2 * offset;

      var rectangle = new Rectangle(left, top, width, height);
      return rectangle;
   }

   public StatusType Status
   {
      get => status;
      set
      {
         status = value;
         if (status is not StatusType.None)
         {
            statusAlpha = 255;
         }

         switch (status)
         {
            case StatusType.Progress:
            {
               var (_, rectangle) = getRectangle(getClientRectangle());
               var pieProgressProcessor = new PieProgressProcessor(rectangle, maximum, getForeColor());
               _pieProgressProcessor = pieProgressProcessor;
               this.Do(Refresh);
               break;
            }
            case StatusType.ProgressStep:
            {
               if (_pieProgressProcessor is (true, var pieProgressProcessor))
               {
                  pieProgressProcessor.Advance();
                  this.Do(Refresh);
               }

               break;
            }
            default:
               _pieProgressProcessor = nil;
               this.Do(() => statusTimer.Enabled = status is not StatusType.None);
               break;
         }
      }
   }

   public void SuccessStatus(string message)
   {
      _successToolTip = message;
      setToolTip();
      Status = StatusType.Success;
   }

   public void FailureStatus(string message)
   {
      _failureToolTip = message;
      setToolTip();
      Status = StatusType.Failure;
   }

   public void ExceptionStatus(Exception exception)
   {
      _exceptionToolTip = exception.Message;
      setToolTip();
      Status = StatusType.Exception;
   }

   protected void drawStatus(Graphics g, Rectangle clientRectangle)
   {
      g.HighQuality();
      switch (status)
      {
         case StatusType.Busy:
         {
            var (radius, rectangle) = getRectangle(clientRectangle);

            (var statusBusyProcessor, _statusBusyProcessor) = _statusBusyProcessor.Create(() => new BusyTextProcessor(getForeColor(), rectangle)
            {
               SpokeThickness = 1, OuterRadius = radius, InnerRadius = radius - 2
            });
            statusBusyProcessor.OnTick();
            statusBusyProcessor.OnPaint(g);

            break;
         }
         case StatusType.Done:
         {
            var (_, rectangle) = getRectangle(clientRectangle);

            var foreColor = Color.White.WithAlpha(statusAlpha);
            var backColor = Color.CadetBlue.WithAlpha(statusAlpha);
            using var brush = new SolidBrush(backColor);
            g.FillRectangle(brush, rectangle);
            using var pen = new Pen(foreColor, 2);
            g.DrawRectangle(pen, rectangle);

            break;
         }
         case StatusType.None or StatusType.Progress or StatusType.ProgressStep:
            break;
         default:
         {
            var (_, rectangle) = getRectangle(clientRectangle);

            var foreColor = getForeColor(status).WithAlpha(statusAlpha);
            var backColor = getBackColor(status).WithAlpha(statusAlpha);
            using var brush = new SolidBrush(backColor);
            g.FillEllipse(brush, rectangle);
            using var pen = new Pen(foreColor, 2);
            g.DrawEllipse(pen, rectangle);

            break;
         }
      }
   }

   protected static int getDiameter(Rectangle clientRectangle)
   {
      var diameter = clientRectangle.Height / 4;
      return diameter < 10 ? 10 : diameter;
   }

   protected static (int radius, Rectangle rectangle) getRectangle(Rectangle clientRectangle)
   {
      var diameter = getDiameter(clientRectangle);
      var top = clientRectangle.Height / 2 - diameter / 2;
      return (diameter / 2, new Rectangle(4, top, diameter, diameter));
   }

   public void ShowAndFadeOut()
   {
      fader.SetTransparentLayeredWindow();
      fader.Start(0);
   }

   protected Rectangle glyphAdjustedClientRectangle()
   {
      var glyphWidth = -WinForms.Controls.SubText.GLYPH_WIDTH;
      var clickGlyph = ClickGlyph ? glyphWidth : 0;
      var chooserGlyph = ChooserGlyph ? glyphWidth : 0;

      return getClientRectangle().OffsetWidth(clickGlyph).OffsetWidth(chooserGlyph);
   }

   public bool UseEmojis { get; set; } = true;

   public void ShowStatus<T>(Maybe<T> _maybe, Either<string, Func<string>> failureMessage) where T : notnull
   {
      if (_maybe)
      {
         Status = StatusType.Success;
      }
      else
      {
         switch (failureMessage)
         {
            case (true, var message, _):
               FailureStatus(message);
               break;
            case (false, _, var func):
               FailureStatus(func());
               break;
         }
      }
   }

   public void ShowStatus<T>(Result<T> _result) where T : notnull
   {
      if (_result)
      {
         Status = StatusType.Success;
      }
      else
      {
         ExceptionStatus(_result.Exception);
      }
   }

   public void ShowStatus<T>(Optional<T> _optional, Either<string, Func<string>> failureMessage) where T : notnull
   {
      if (_optional)
      {
         Status = StatusType.Success;
      }
      else if (_optional.Exception is (true, var exception))
      {
         ExceptionStatus(exception);
      }
      else
      {
         switch (failureMessage)
         {
            case (true, var message, _):
               FailureStatus(message);
               break;
            case (false, _, var func):
               FailureStatus(func());
               break;
         }
      }
   }

   public long ObjectId { get; set; }

   public DividerValidation DividerValidation
   {
      get => dividerValidation;
      set
      {
         dividerValidation = value;
         Invalidate();
      }
   }
}