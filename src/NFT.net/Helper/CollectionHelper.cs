﻿// <copyright file="CollectionHelper.cs" company="Tedeschi">
// Copyright (c) Tedeschi. All rights reserved.
// </copyright>

namespace Tedeschi.NFT.Helper
{
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using Tedeschi.NFT.Model;

    public class CollectionHelper
    {
        public static void Create(string layersFolder, string outputFolder, int metadataType, string metadataDescription, string metadataImageBaseUri, int collectionSize, int collectionInitialNumber, string collectionImagePrefix)
        {
            var layers = LayerHelper.Load(layersFolder);

            var imageDescriptors = GeneratorHelper.Create(layers, collectionSize);
            var collectionNumber = collectionInitialNumber;
            var metadataList = new List<Metadata>();

            var imagesFoder = $"{outputFolder}\\{Constants.ImagesFolderName}";

            if (!Directory.Exists(imagesFoder))
            {
                Directory.CreateDirectory(imagesFoder);
            }

            foreach (var item in imageDescriptors)
            {
                var combinedImages = ImageHelper.Combine(item.Files.ToArray());
                var filename = $"{collectionNumber}{Constants.FileExtension.Png}";
                var filenameWithoutExtension = $"{collectionNumber}";

                if (!string.IsNullOrWhiteSpace(collectionImagePrefix))
                {
                    filename = $"{collectionImagePrefix}{collectionNumber}{Constants.FileExtension.Png}";
                    filenameWithoutExtension = $"{collectionImagePrefix}{collectionNumber}";
                }

                var metadata = new Metadata
                {
                    Id = collectionNumber,
                    Dna = item.Dna,
                    Name = filenameWithoutExtension,
                    Filename = System.Uri.EscapeDataString(filename),
                    Description = metadataDescription,
                    Image = $"{metadataImageBaseUri}/{System.Uri.EscapeDataString(filename)}",
                    Attributes = item.Attributes.Select(i => new Attribute { Layer = i.Layer, Value = i.Value }).ToList(),
                };

                metadataList.Add(metadata);

                combinedImages.Save($"{imagesFoder}\\{filename}", ImageFormat.Png);

                // Disposing image
                combinedImages?.Dispose();

                collectionNumber++;
            }

            MetadataHelper.GenerateMetadata(outputFolder, metadataList, metadataType);
        }
    }
}
