#if UNITY_EDITOR && UNITY_ANDROID

using UnityEditor;

using GooglePlayServices;

namespace GooglePlayGames.Editor {

    [InitializeOnLoad]
    internal static class DependencyResolver {

        static DependencyResolver()
        {
            EditorApplication.delayCall += () => {
                PlayServicesResolver.Resolve(null, false, null);
            };
        }

    }

}

#endif