using UnityEngine;

namespace OctanGames.Tutorials.MeshCombine
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MeshCombiner : MonoBehaviour
    {
        [SerializeField] private Material _material;

        private void Start()
        {
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            var combine = new CombineInstance[meshFilters.Length];

            for (var i = 0; i < meshFilters.Length; i++)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);
            }

            var mesh = new Mesh();
            mesh.CombineMeshes(combine);
            GetComponent<MeshFilter>().sharedMesh = mesh;
            GetComponent<MeshRenderer>().material = _material;
            gameObject.SetActive(true);
        }
    }
}
