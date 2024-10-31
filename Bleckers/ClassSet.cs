using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bleckers;
public class ClassSet {
    private HashSet<string> _classes = new HashSet<string>();

    public ClassSet(string initial) {
        foreach (var cname in initial.Split(' ')) {
            _classes.Add(cname);
        }
    }

    public ClassSet(string?[] initial) {
        foreach (var cname in initial.Where(x => x != null)) {
            _classes.Add(cname!);
        }
    }

    public void Add(string classes) {
        foreach (var cname in classes.Split(' ')) {
            _classes.Add(cname);
        }
    }

    public override string ToString() => string.Join(" ", _classes);
}
