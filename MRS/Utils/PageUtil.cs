using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace MRS.Utils
{
    public static class PageUtil
    {
        public static PageModel<U> ToPageList<U, T>(this IEnumerable<T> list, int index = 1, int pageSize = 5)
        {
            int total = list.Count();
            if (total == 0) return new PageModel<U>
            {
                Index = index,
                Left = 0,
                Right = 0,
                List = new List<U>(),
                Total = 0
            };
            list = list.Skip((index - 1) * pageSize).Take(pageSize);
            List<T> data = list.ToList();
            List<U> result = new List<U>();
            foreach (var item in data)
            {
                result.Add(item.Adapt<U>());
            }
            int left = Math.Max(index - 2, 1);
            int right = Math.Min(left + 4, (int)Math.Ceiling(1.0 * total / pageSize));
            left = Math.Max(1, Math.Min(right - 4, left));

            return new PageModel<U>
            {
                Index = index,
                Left = left,
                Right = right,
                List = result,
                Total = total
            };
        }

        public static PageModel<U> ToPageList<U>(this List<U> list, int index = 1, int pageSize = 5)
        {
            int total = list.Count();
            if (total == 0) return new PageModel<U>
            {
                Index = index,
                Left = 0,
                Right = 0,
                List = new List<U>(),
                Total = 0
            };
            List<U> result = list.Skip((index - 1) * pageSize).Take(pageSize).ToList();
            int left = Math.Max(index - 2, 1);
            int right = Math.Min(left + 4, (int)Math.Ceiling(1.0 * total / pageSize));
            left = Math.Max(1, Math.Min(right - 4, left));

            return new PageModel<U>
            {
                Index = index,
                Left = left,
                Right = right,
                List = result,
                Total = total
            };
        }

        public class PageModel<T>
        {
            public ICollection<T> List { get; set; }

            public int Index { get; set; }
            public int Left { get; set; }
            public int Right { get; set; }
            public int Total { get; set; }
        }
    }
}
