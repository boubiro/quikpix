//Source: http://www.solidrockstable.com/blogs/PragmaticTSQL/Lists/Posts/Post.aspx?ID=37
//Author: Greg E. Wilson

namespace QuikPix.Controls
{
    public static class ExtensionMethods
    {
        public static bool Contains(this string[] stringList, string item)
        {
            foreach (string s in stringList)
                if (item.Equals(s))
                    return true;
            return false;
        }
    }
}