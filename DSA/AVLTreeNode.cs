using System;

namespace AVLTree
{
    /// <summary>
    /// An AVL tree node class
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    public class AVLTreeNode<TNode> : IComparable<TNode>
        where TNode : IComparable
    {
        private readonly AVLTree<TNode> _tree;
        public AVLTreeNode(TNode value, AVLTreeNode<TNode> parent, AVLTree<TNode> tree)
        {
            Value = value;
            Parent = parent;
            _tree = tree;
        }

        private TreeState State
        {
            get
            {
                if (LeftHeight - RightHeight > 1)
                {
                    return TreeState.LeftHeavy;
                }

                return RightHeight - LeftHeight > 1 ? TreeState.RightHeavy : TreeState.Balanced;
            }
        }

        private int BalanceFactor => RightHeight - LeftHeight;

        internal void Balance()
        {
            switch (State)
            {
                case TreeState.RightHeavy:
                    if (Right != null && Right.BalanceFactor < 0)
                    {
                        LeftRightRotation();
                    }
                    else
                    {
                        LeftRotation();
                    }
                    break;
                case TreeState.LeftHeavy:
                    if (Left != null && Left.BalanceFactor > 0)
                    {
                        RightLeftRotation();
                    }
                    else
                    {
                        RightRotation();
                    }
                    break;
                case TreeState.Balanced:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void LeftRotation()
        {
            //     a
            //      \
            //       b
            //        \
            //         c
            //
            // becomes
            //       b
            //      / \
            //     a   c

            var newRoot = Right;

            // replace the current root with the new root
            ReplaceRoot(newRoot);

            // take ownership of right's left child as right (now parent)
            Right = newRoot.Left;

            // the new root takes this as it's left
            newRoot.Left = this;
        }

        private void RightRotation()
        {
            //     c (this)
            //    /
            //   b
            //  /
            // a
            //
            // becomes
            //       b
            //      / \
            //     a   c

            var newRoot = Left;

            // replace the current root with the new root
            ReplaceRoot(newRoot);

            // take ownership of left's right child as left (now parent)
            Left = newRoot.Right;

            // the new root takes this as it's right
            newRoot.Right = this;
        }

        private void ReplaceRoot(AVLTreeNode<TNode> newRoot)
        {
            if (Parent != null)
            {
                if (Parent.Left == this)
                {
                    Parent.Left = newRoot;
                }
                else if (Parent.Right == this)
                {
                    Parent.Right = newRoot;
                }
            }
            else
            {
                _tree._head = newRoot;
            }

            newRoot.Parent = Parent;
            Parent = newRoot;
        }

        private void LeftRightRotation()
        {
            Right.RightRotation();
            LeftRotation();
        }

        private void RightLeftRotation()
        {
            Left.LeftRotation();
            RightRotation();
        }

        private static int MaxChildHeight(AVLTreeNode<TNode> node)
        {
            if (node != null)
            {
                return 1 + Math.Max(MaxChildHeight(node.Left), MaxChildHeight(node.Right));
            }

            return 0;
        }

        private int LeftHeight => MaxChildHeight(Left);

        private int RightHeight => MaxChildHeight(Right);

        private AVLTreeNode<TNode> _left;
        public AVLTreeNode<TNode> Left 
        { 
            get
            {
                return _left;
            }
            internal set
            {
                _left = value;
                if (_left != null)
                {
                    _left.Parent = this;
                }
            }
        }

        private AVLTreeNode<TNode> _right;
        public AVLTreeNode<TNode> Right
        { 
            get
            {
                return _right;
            }
            internal set
            {
                _right = value;
                if (_right != null)
                {
                    _right.Parent = this;
                }
            }
        }

        public AVLTreeNode<TNode> Parent { get; internal set; }
        public TNode Value { get; }

        /// <summary>
        /// Compares the current node to the provided value
        /// </summary>
        /// <param name="other">The node value to compare to</param>
        /// <returns>1 if the instance value is greater than the provided value, -1 if less or 0 if equal.</returns>
        public int CompareTo(TNode other)
        {
            return Value.CompareTo(other);
        }
    }
	
    public enum TreeState
    {
        Balanced,
        LeftHeavy,
        RightHeavy,
    }

}
