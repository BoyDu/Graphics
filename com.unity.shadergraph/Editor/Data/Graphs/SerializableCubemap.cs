using System;
using UnityEngine;

namespace UnityEditor.ShaderGraph.Internal
{
    [Serializable]
    public sealed class SerializableCubemap : ISerializationCallbackReceiver
    {
        [SerializeField]
        string m_SerializedCubemap;

        [SerializeField]
        string m_Guid;

        [NonSerialized]
        Cubemap m_Cubemap;

        [Serializable]
        class CubemapHelper
        {
#pragma warning disable 649
            public Cubemap cubemap;
#pragma warning restore 649
        }

        // used to load a Texture2DArray ref guid without loading the texture
        [Serializable]
        class MinimalCubemapHelper
        {
            [Serializable]
            public struct MinimalTextureRef
            {
                public string guid;
            }
            public MinimalTextureRef cubemap;
        }

        internal string guid
        {
            get
            {
                if (!string.IsNullOrEmpty(m_SerializedCubemap))
                {
                    var textureHelper = new MinimalCubemapHelper();
                    EditorJsonUtility.FromJsonOverwrite(m_SerializedCubemap, textureHelper);
                    if (!string.IsNullOrEmpty(textureHelper.cubemap.guid))
                        return textureHelper.cubemap.guid;
                }
                if (!string.IsNullOrEmpty(m_Guid))
                {
                    return m_Guid;
                }
                if (m_Cubemap != null)
                {
                    if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(m_Cubemap, out string guid, out long localId))
                        return guid;
                }
                return null;
            }
        }

        public Cubemap cubemap
        {
            get
            {
                if (!string.IsNullOrEmpty(m_SerializedCubemap))
                {
                    var textureHelper = new CubemapHelper();
                    EditorJsonUtility.FromJsonOverwrite(m_SerializedCubemap, textureHelper);
                    m_SerializedCubemap = null;
                    m_Guid = null;
                    m_Cubemap = textureHelper.cubemap;
                }
                else if (!string.IsNullOrEmpty(m_Guid) && m_Cubemap == null)
                {
                    m_Cubemap = AssetDatabase.LoadAssetAtPath<Cubemap>(AssetDatabase.GUIDToAssetPath(m_Guid));
                    m_Guid = null;
                }

                return m_Cubemap;
            }
            set
            {
                m_Cubemap = value;
                m_Guid = null;
                m_SerializedCubemap = null;
            }
        }

        public void OnBeforeSerialize()
        {
            m_SerializedCubemap = EditorJsonUtility.ToJson(new CubemapHelper { cubemap = cubemap }, false);
        }

        public void OnAfterDeserialize()
        {
        }
    }
}
