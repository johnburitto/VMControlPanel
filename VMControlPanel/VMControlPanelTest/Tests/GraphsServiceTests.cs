using Core.Dtos;
using Infrastructure.Services.Impls;

namespace VMControlPanelTest.Tests
{
    public class GraphsServiceTests
    {
        [Fact]
        public void CreateGraph_NormalFlow()
        {
            // Give
            var dto = new GraphDto
            {
                Height = 100,
                Width = 100,
                Name = "Graph",
                Labels = [],
                Values = [],
                UserId = "1"
            };

            // When
            var result = GraphsService.CreateGraph(GraphType.Donut, dto);

            // Then
            Assert.NotEmpty(result);
            Assert.Contains(FileManager.FileDirectory, result);
            Assert.Contains(dto.UserId, result);
            Assert.Contains(dto.Name, result);
            Assert.True(File.Exists(result));
        }
        
        [Fact]
        public void CreateGraph_NoUserId()
        {
            // Give
            var dto = new GraphDto
            {
                Height = 100,
                Width = 100,
                Name = "Graph",
                Labels = [],
                Values = []
            };

            // When
            var result = GraphsService.CreateGraph(GraphType.Donut, dto);

            // Then
            Assert.NotEmpty(result);
            Assert.Contains(FileManager.FileDirectory, result);
            Assert.DoesNotContain("1", result);
            Assert.Contains(dto.Name, result);
            Assert.True(File.Exists(result));
        }
        
        [Fact]
        public void CreateGraph_NoName()
        {
            // Give
            var dto = new GraphDto
            {
                Height = 100,
                Width = 100,
                Labels = [],
                Values = [],
                UserId = "1"
            };

            // When
            var result = GraphsService.CreateGraph(GraphType.Donut, dto);

            // Then
            Assert.NotEmpty(result);
            Assert.Contains(FileManager.FileDirectory, result);
            Assert.Contains(dto.UserId, result);
            Assert.DoesNotContain("Graph", result);
            Assert.True(File.Exists(result));
        }
        
        [Fact]
        public void CreateGraph_NoNameNoUserId()
        {
            // Give
            var dto = new GraphDto
            {
                Height = 100,
                Width = 100,
                Labels = [],
                Values = []
            };

            // When
            var result = GraphsService.CreateGraph(GraphType.Donut, dto);

            // Then
            Assert.NotEmpty(result);
            Assert.Contains(FileManager.FileDirectory, result);
            Assert.DoesNotContain("1", result);
            Assert.DoesNotContain("Graph", result);
            Assert.True(File.Exists(result));
        }

        [Fact]
        public void DeleteGraphFromLocal_NormalFlow()
        {
            // Give
            var normalGraph = $"{FileManager.FileDirectory}/1/Graph.png";
            var noUserIdGraph = $"{FileManager.FileDirectory}/Graph.png";
            var noNameGraph = $"{FileManager.FileDirectory}/1/.png";
            var noNameNoUserIdGraph = $"{FileManager.FileDirectory}/.png";

            // When
            GraphsService.DeleteGraphFromLocal(normalGraph);
            GraphsService.DeleteGraphFromLocal(noUserIdGraph);
            GraphsService.DeleteGraphFromLocal(noNameGraph);
            GraphsService.DeleteGraphFromLocal(noNameNoUserIdGraph);

            // Then
            Assert.False(File.Exists(normalGraph));
            Assert.False(File.Exists(noUserIdGraph));
            Assert.False(File.Exists(noNameGraph));
            Assert.False(File.Exists(noNameNoUserIdGraph));
        }
    }
}
