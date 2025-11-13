using System.Collections.Generic;
using UnityEngine;

public static class RepulsionSystem
{
    public static void Apply(IReadOnlyList<HashSet<Atom>> molecules, float minDist, float strength)
    {
        foreach (var mol in molecules)
        {
            foreach (var a in mol)
            {
                foreach (var b in mol)
                {
                    if (a == b) continue;

                    // Ignore linked atoms
                    if (a.linkedAtoms.Contains(b)) continue;

                    Vector3 delta = b.transform.position - a.transform.position;
                    float dist = delta.magnitude;

                    if (dist < minDist && dist > 0.0001f)
                    {
                        Vector3 force = delta.normalized * (strength / (dist * dist));
                        a.rb.AddForce(-force);
                        b.rb.AddForce(force);
                    }
                }
            }
        }
    }
}
