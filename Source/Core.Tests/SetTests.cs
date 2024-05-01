using Core.Assertions;
using Core.Collections;
using Core.Numbers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Assertions.AssertionFunctions;

namespace Core.Tests;

[TestClass]
public class SetTests
{
   [TestMethod]
   public void SetOfStringVsStringSetTest()
   {
      Set<string> setOfString = ["Case"];
      StringSet stringSet = ["Case"];

      setOfString.Remove("case");
      setOfString.Must().HaveCountOfExactly(1).OrThrow();

      stringSet.Remove("case");
      setOfString.Must().HaveCountOfExactly(0).OrThrow();
   }

   [TestMethod]
   public void SetOperatorsTest()
   {
      Set<int> union1 = [.. 0.Until(5).Select(i => i * 2)];
      Set<int> union2 = [.. 0.Until(5).Select(i => i * 2 + 1)];
      var union = union1 | union2;
      Console.WriteLine($"({enumerableImage(union1)}) union ({enumerableImage(union2)}) = ({enumerableImage(union)})");

      Set<int> intersection1 = [3, 4];
      Set<int> intersection2 = [1, 3, 5, 2, 4];
      var intersection = intersection1 & intersection2;
      Console.WriteLine($"({enumerableImage(intersection1)}) intersection ({enumerableImage(intersection2)}) = ({enumerableImage(intersection)})");

      Set<int> exception1 = [.. 3.Until(10)];
      Set<int> exception2 = [.. 0.Until(6)];
      var exception = exception1 - exception2;
      Console.WriteLine($"({enumerableImage(exception1)}) exception ({enumerableImage(exception2)}) = ({enumerableImage(exception)})");

      var symmetricException = intersection1 ^ intersection2;
      Console.WriteLine(
         $"({enumerableImage(intersection1)}) symmetric exception ({enumerableImage(intersection2)}) = ({enumerableImage(symmetricException)})");
   }

   [TestMethod]
   public void SetOperators2Test()
   {
      StringSet indexes =
      [
         "BillingRecordTypeId", "BillingCompanyId", "BillingRecordStatusId", "CommercialAssetId",
         "DistributionRecordId", "TariffFeeHeaderId", "ContractId", "ContractRateSheetFeeHeaderId", "BillingScheduleLineItemId", "InvoiceDetailId",
         "FlowDate", "IsEstimated"
      ];
      StringSet found = ["InvoiceDetailId", "IsEstimated", "BillingAmount"];

      var result = indexes ^ found;
      Console.WriteLine($"^{enumerableImage(result.OrderBy(i => i))}");

      result = indexes - found;
      Console.WriteLine($"-{enumerableImage(result.OrderBy(i => i))}");

      result = indexes & found;
      Console.WriteLine($"&{enumerableImage(result.OrderBy(i => i))}");
   }
}