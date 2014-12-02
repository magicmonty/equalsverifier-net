namespace EqualsVerifier.TestHelpers.Types
{
    public static class RecursiveTypeHelper
    {
        public sealed class Node
        {
            public Node node;
        }

        public sealed class NodeArray
        {
            public NodeArray[] nodeArrays;
        }

        public sealed class TwoStepNodeA
        {
            public TwoStepNodeB node;
        }

        public sealed class TwoStepNodeB
        {
            public TwoStepNodeA node;
        }

        public sealed class TwoStepNodeArrayA
        {
            public TwoStepNodeArrayB[] nodes;
        }

        public sealed class TwoStepNodeArrayB
        {
            public TwoStepNodeArrayA[] nodes;
        }

        public sealed class NotRecursiveA
        {
            public NotRecursiveB b;
            public NotRecursiveC c;
        }

        public sealed class NotRecursiveB
        {
            public NotRecursiveD d;
        }

        public sealed class NotRecursiveC
        {
            public NotRecursiveD d;
        }

        public sealed class NotRecursiveD
        {

        }

        public sealed class RecursiveWithAnotherFieldFirst
        {
            public RecursiveThisIsTheOtherField point;
            public RecursiveWithAnotherFieldFirst recurse;
        }

        public sealed class RecursiveThisIsTheOtherField
        {

        }
    }
}

