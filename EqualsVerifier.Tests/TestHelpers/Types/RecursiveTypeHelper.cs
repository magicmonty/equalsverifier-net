using System;

namespace EqualsVerifier.Tests.TestHelpers.Types
{
    public static class RecursiveTypeHelper
    {
        public static sealed class Node
        {
            Node node;
        }

        public static sealed class NodeArray
        {
            NodeArray[] nodeArrays;
        }

        public static sealed class TwoStepNodeA
        {
            TwoStepNodeB node;
        }

        public static sealed class TwoStepNodeB
        {
            TwoStepNodeA node;
        }

        public static sealed class TwoStepNodeArrayA
        {
            TwoStepNodeArrayB[] nodes;
        }

        public static sealed class TwoStepNodeArrayB
        {
            TwoStepNodeArrayA[] nodes;
        }

        public static sealed class NotRecursiveA
        {
            NotRecursiveB b;
            NotRecursiveC c;
        }

        public static sealed class NotRecursiveB
        {
            NotRecursiveD d;
        }

        public static sealed class NotRecursiveC
        {
            NotRecursiveD d;
        }

        public static sealed class NotRecursiveD
        {

        }

        public static sealed class RecursiveWithAnotherFieldFirst
        {
            RecursiveThisIsTheOtherField point;
            RecursiveWithAnotherFieldFirst recurse;
        }

        public static sealed class RecursiveThisIsTheOtherField
        {

        }
    }
}

