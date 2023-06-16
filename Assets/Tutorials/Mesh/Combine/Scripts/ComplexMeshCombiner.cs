using UnityEngine;

namespace OctanGames.Tutorials.Mesh.Combine.Scripts
{
    public class ComplexMeshCombiner : MonoBehaviour
    {
        [SerializeField] private Material _material;

        private void Start()
        {
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            int totalMeshes = meshFilters.Length + skinnedMeshRenderers.Length;
            var combine = new CombineInstance[totalMeshes];

            for (var i = 0; i < meshFilters.Length; i++)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                int index = meshFilters.Length + i;
                combine[index].mesh = skinnedMeshRenderers[i].sharedMesh;
                combine[index].transform = skinnedMeshRenderers[i].transform.localToWorldMatrix;
                skinnedMeshRenderers[i].gameObject.SetActive(false);
            }

            var mesh = new UnityEngine.Mesh();
            mesh.CombineMeshes(combine, true, true);

            gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
            gameObject.AddComponent<MeshRenderer>().material = _material;
            gameObject.SetActive(true);
        }
    }
}