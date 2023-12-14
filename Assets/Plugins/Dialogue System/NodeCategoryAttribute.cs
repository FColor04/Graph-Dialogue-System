using System;

namespace Dialogue_System
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeCategoryAttribute : Attribute
    {
        public string Category;

        public NodeCategoryAttribute(string category)
        {
            Category = category;
        }
    }
}