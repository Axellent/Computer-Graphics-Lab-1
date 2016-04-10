using GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAJ_Lab1
{
    public class Heightmap
    {
        GraphicsDevice graphicsDevice;
        BasicEffect effect;
        Texture2D heightmapTexture;
        Entity heightmapEntity;

        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        HeightmapSystem.VertexPositionColorNormal[] vertices;
        int[] indices;
        float[,] heightData;

        int terrainWidth = 4;
        int terrainHeight = 3;
        float indicesLenDiv3 = 1;

        public Heightmap(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            effect = new BasicEffect(graphicsDevice);
            effect.EnableDefaultLighting();

            heightmapEntity = EntityFactory.Instance.NewEntityWithTag("heightmap");
        }

        public void LoadHeightmap(Texture2D heightmapTexture, Matrix viewMatrix, Matrix projectionMatrix)
        {
            this.heightmapTexture = heightmapTexture;

            heightData = HeightmapSystem.GetHeightData(heightmapTexture, ref terrainWidth, ref terrainHeight);
            vertices = HeightmapSystem.GetVertices(terrainWidth, terrainHeight, heightData);
            indices = HeightmapSystem.GetIndices(terrainWidth, terrainHeight);
            indicesLenDiv3 = indices.Length / 3;
            HeightmapSystem.TransformToNormals(ref vertices, indices);
            HeightmapSystem.CopyToBuffers(ref vertexBuffer, ref indexBuffer, vertices, indices, graphicsDevice);

            HeightmapComponent heightmapComponent = new HeightmapComponent(graphicsDevice, effect, heightmapTexture);
            heightmapComponent.SetMatrices(viewMatrix, projectionMatrix);
            heightmapComponent.UpdateHeightmap(vertexBuffer, indexBuffer, terrainWidth, terrainHeight, indicesLenDiv3);

            ComponentManager.Instance.AddComponentToEntity(heightmapEntity, heightmapComponent);
            SystemManager.Instance.RegisterSystem("render", new HeightmapSystem());
        }
    }
}
