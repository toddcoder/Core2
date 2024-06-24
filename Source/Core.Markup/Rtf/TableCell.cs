using System.Text;
using Core.Monads;
using Core.Objects;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf;

public class TableCell(float width, int rowIndex, int columnIndex, Table parentTable) : BlockList(true, false)
{
   private Alignment horizontalAlignment = Alignment.None;
   private VerticalAlignment verticalAlignmentAlignment = VerticalAlignment.Top;
   private Borders borders = new();
   private LateLazy<CellMergeInfo> mergeInfo = new(true, "Merge info has not be set through MergeInfo property");

   public bool IsBeginOfColumnSpan => mergeInfo.AnyValue.Map(mergeInfo => mergeInfo.ColumnIndex == 0) | false;

   public bool IsBeginOfRowSpan => mergeInfo.AnyValue.Map(mergeInfo => mergeInfo.RowIndex == 0) | false;

   public bool IsMerged => mergeInfo.IsActivated;

   public CellMergeInfo MergeInfo
   {
      get => mergeInfo.Value;
      set
      {
         mergeInfo.ActivateWith(() => value);
         _ = mergeInfo.Value;
      }
   }

   public float Width
   {
      get => width;
      set => width = value;
   }

   public Borders Borders => borders;

   public Table ParentTable { get; } = parentTable;

   public Maybe<ColorDescriptor> BackgroundColor { get; set; } = nil;

   public Alignment Alignment
   {
      get => horizontalAlignment;
      set => horizontalAlignment = value;
   }

   public VerticalAlignment AlignmentVerticalAlignment
   {
      get => verticalAlignmentAlignment;
      set => verticalAlignmentAlignment = value;
   }

   public int RowIndex => rowIndex;

   public int ColumnIndex => columnIndex;

   public float OuterLeftBorderClearance { get; set; }

   public void SetBorderColor(ColorDescriptor color)
   {
      borders[Direction.Top].Color = color;
      borders[Direction.Bottom].Color = color;
      borders[Direction.Left].Color = color;
      borders[Direction.Right].Color = color;
   }

   public override string Render()
   {
      var result = new StringBuilder();

      var align = horizontalAlignment switch
      {
         Alignment.Left => @"\ql",
         Alignment.Right => @"\qr",
         Alignment.Center => @"\qc",
         Alignment.FullyJustify => @"\qj",
         Alignment.Distributed => @"\qd",
         _ => string.Empty
      };

      if (blocks.Count <= 0)
      {
         result.AppendLine(@"\pard\intbl");
      }
      else
      {
         for (var i = 0; i < blocks.Count; i++)
         {
            var block = blocks[i];
            block.DefaultCharFormat.CopyFrom(defaultCharFormat);

            if (block.Margins[Direction.Top] < 0)
            {
               block.Margins[Direction.Top] = 0;
            }

            if (block.Margins[Direction.Right] < 0)
            {
               block.Margins[Direction.Right] = 0;
            }

            if (block.Margins[Direction.Bottom] < 0)
            {
               block.Margins[Direction.Bottom] = 0;
            }

            if (block.Margins[Direction.Left] < 0)
            {
               block.Margins[Direction.Left] = 0;
            }

            if (i == 0)
            {
               block.BlockHead = $@"\pard\intbl{align}";
            }
            else
            {
               block.BlockHead = $@"\par{align}";
            }

            block.BlockTail = string.Empty;
            result.AppendLine(block.Render());
         }
      }

      result.AppendLine(@"\cell");
      return result.ToString();
   }
}