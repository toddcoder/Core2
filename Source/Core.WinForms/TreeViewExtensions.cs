namespace Core.WinForms;

public static class TreeViewExtensions
{
   public static IEnumerable<TreeNode> AllNodes(this TreeView treeView, bool includeChildren)
   {
      if (includeChildren)
      {
         foreach (TreeNode node in treeView.Nodes)
         {
            yield return node;

            foreach (var childNode in childNodes(node))
            {
               yield return childNode;
            }
         }
      }
      else
      {
         foreach (TreeNode node in treeView.Nodes)
         {
            yield return node;
         }
      }

      yield break;

      static IEnumerable<TreeNode> childNodes(TreeNode node)
      {
         foreach (TreeNode childNode in node.Nodes)
         {
            yield return childNode;

            foreach (var subNode in childNodes(childNode))
            {
               yield return subNode;
            }
         }
      }
   }

   public static IEnumerable<TreeNode> AllNodes(this TreeView treeView, Func<TreeNode, bool> predicate, bool includeChildren)
   {
      if (includeChildren)
      {
         foreach (TreeNode node in treeView.Nodes)
         {
            if (predicate(node))
            {
               yield return node;
            }

            foreach (var childNode in childNodes(node))
            {
               yield return childNode;
            }
         }
      }
      else
      {
         foreach (TreeNode node in treeView.Nodes)
         {
            if (predicate(node))
            {
               yield return node;
            }
         }
      }

      yield break;

      IEnumerable<TreeNode> childNodes(TreeNode node)
      {
         foreach (TreeNode childNode in node.Nodes)
         {
            if (predicate(childNode))
            {
               yield return childNode;
            }

            foreach (var subNode in childNodes(childNode))
            {
               if (predicate(childNode))
               {
                  yield return subNode;
               }
            }
         }
      }
   }
}