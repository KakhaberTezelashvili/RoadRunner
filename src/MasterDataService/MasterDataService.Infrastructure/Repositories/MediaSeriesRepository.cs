using Dapper;
using MasterDataService.Shared.Constants.Media;
using MasterDataService.Shared.Dtos.Media;
using Microsoft.Data.SqlClient;
using TDOC.Common.Data.Constants.Media;
using TDOC.Common.Data.Enumerations.Media;

namespace MasterDataService.Infrastructure.Repositories;

/// <inheritdoc cref="IMediaSeriesRepository" />
public class MediaSeriesRepository : RepositoryBase<PictureRefModel>, IMediaSeriesRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediaSeriesRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public MediaSeriesRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<IList<MediaEntryDto>> GetSeriesByKeyIdAndLinkTypeAndSeriesTypeAsync(int keyId, string linkType, int seriesType)
    {
        SqlConnection connection = GetSqlConnection();

        IEnumerable<MediaSeriesDto> associatedMedia;

        string link = linkType.ToLower();

        if (link == MediaSeriesLinks.Unit || link == MediaSeriesLinks.Product)
        {
            // For units and products we lookup all media associated with the unit, product and item
            int? unitKeyId = null;
            int? productKeyId = null;
            if (link == MediaSeriesLinks.Unit)
            {
                unitKeyId = keyId;
            }
            else
            {
                productKeyId = keyId;
            }

            associatedMedia = await connection.QueryAsync<MediaSeriesDto>(
                SqlQueryForUnitsAndProducts,
                new
                {
                    UnitKeyId = unitKeyId,
                    ProductKeyId = productKeyId,
                    SeriesType = seriesType,
                    LinkTypeUnit = MediaSeriesLinks.Unit,
                    LinkTypeProduct = MediaSeriesLinks.Product,
                    LinkTypeItem = MediaSeriesLinks.Item
                }
            );
        }
        else
        {
            // Translate link type into the reference column name
            string columnName = MediaSeriesReferenceColumn.FromLinkType(linkType);

            // Get media series
            associatedMedia = await connection.QueryAsync<MediaSeriesDto>(
                GetDefaultSqlQuery(columnName),
                new
                {
                    KeyId = keyId,
                    SeriesType = seriesType,
                    LinkType = link
                }
            );
        }

        var mediaList = new List<MediaEntryDto>();

        foreach (MediaSeriesDto dto in associatedMedia)
        {
            var entry = new MediaEntryDto
            {
                Position = dto.PictNo,
                KeyId = dto.PicsKeyId,
                LinkType = dto.LinkType
            };

            AddMediaItem(entry, dto.HasImage, MediaFormat.Image.ToString(), string.Empty);
            AddMediaItem(entry, dto.HasSound, MediaFormat.Audio.ToString(), string.Empty);
            AddMediaItem(entry, dto.HasText, MediaFormat.Text.ToString(), string.Empty);
            AddMediaItem(entry, dto.HasThumbnail, MediaFormat.Image.ToString(), TDocMediaIdentifiers.Thumbnail);
            AddMediaItem(entry, dto.HasVideo, MediaFormat.Video.ToString(), string.Empty,
                Path.GetExtension(dto.PicsOrgFilename));

            mediaList.Add(entry);
        }

        return mediaList;
    }

    /// <summary>
    /// Contains the names of columns that reference the object that a media series is linked to.
    /// </summary>
    private SqlConnection GetSqlConnection() => _context.Database.GetDbConnection() as SqlConnection;

    /// <summary>
    /// Internal DTO for retrieving media series data from the database.
    /// </summary>
    /// <summary>
    /// If <paramref name="hasMediaOfThisType"/> is <c>true</c>, adds a new media item to the
    /// entry. Otherwise, does nothing.
    /// </summary>
    /// <param name="mediaEntry">Media entry object.</param>
    /// <param name="hasMediaOfThisType">
    /// If true, a new <see cref="MediaItemDto"/> instance is added to the <see cref="Types"/> collection.
    /// </param>
    /// <param name="type">
    /// The type of media; refer to <see cref="MediaFormat"/> for valid values.
    /// </param>
    /// <param name="identifier">
    /// Domain or system specific identifier providing further context for the media.
    /// </param>
    /// <param name="format">Format of the media (currently only used for video).</param>
    private static void AddMediaItem(MediaEntryDto mediaEntry, bool hasMediaOfThisType, string type, string identifier,
        string format = "")
    {
        if (hasMediaOfThisType)
        {
            mediaEntry.Types.Add(new MediaItemDto
            {
                Type = type,
                Identifier = identifier,
                Format = format
            });
        }
    }

    #region SQL queries for media series

    private static string GetDefaultSqlQuery(string referenceColumnName)
    {
        return $@"
                SELECT
	                  PICTNO
	                , PICSKEYID
	                , CASE
		                WHEN PICSPICTURE IS NULL THEN 0
		                ELSE 1
	                  END AS HASIMAGE
	                , CASE
		                WHEN PICSSOUND IS NULL THEN 0
		                ELSE 1
	                  END AS HASSOUND
	                , CASE
		                WHEN PICSTEXT IS NULL THEN 0
		                ELSE 1
	                  END AS HASTEXT
	                , CASE
		                WHEN PICSTHUMBNAIL IS NULL THEN 0
		                ELSE 1
	                  END AS HASTHUMBNAIL
	                , CASE
		                WHEN PICSVIDEO IS NULL THEN 0
		                ELSE 1
	                  END AS HASVIDEO
                    , PICSORGFILENAME
                    , @LinkType AS LINKTYPE
                FROM
	                TPICTREF
                JOIN
	                TPICS
                ON
	                PICTPICSKEYID = PICSKEYID
                WHERE
                    {referenceColumnName} = @KeyId
                    AND
                    PICTSERIES = @SeriesType
                ORDER BY
                    PICTNO
            ";
    }

    private const string SqlQueryForUnitsAndProducts = @"
			DECLARE @ITEMKEYID INT = NULL;

			IF (NOT @UNITKEYID IS NULL)
			BEGIN
				SELECT
					@PRODUCTKEYID = PRODKEYID, @ITEMKEYID = ITEMKEYID
				FROM
					TUNIT
				JOIN
					TPRODUCT
				ON
					UNITPRODKEYID = PRODKEYID
				JOIN
					TITEM
				ON
					PRODITEMKEYID = ITEMKEYID
				WHERE
					UNITUNIT = @UNITKEYID
			END
			ELSE IF (NOT @PRODUCTKEYID IS NULL)
			BEGIN
				SELECT
					@ITEMKEYID = ITEMKEYID
				FROM
					TPRODUCT
				JOIN
					TITEM
				ON
					PRODITEMKEYID = ITEMKEYID
				WHERE
					PRODKEYID = @PRODUCTKEYID
			END;

            SELECT
	                PICTNO
	            , PICSKEYID
				, PICTREFUNITUNIT
				, PICTREFPRODKEYID
				, PICTREFITEMKEYID
	            , CASE
		            WHEN PICSPICTURE IS NULL THEN 0
		            ELSE 1
	                END AS HASIMAGE
	            , CASE
		            WHEN PICSSOUND IS NULL THEN 0
		            ELSE 1
	                END AS HASSOUND
	            , CASE
		            WHEN PICSTEXT IS NULL THEN 0
		            ELSE 1
	                END AS HASTEXT
	            , CASE
		            WHEN PICSTHUMBNAIL IS NULL THEN 0
		            ELSE 1
	                END AS HASTHUMBNAIL
	            , CASE
		            WHEN PICSVIDEO IS NULL THEN 0
		            ELSE 1
	                END AS HASVIDEO
                , PICSORGFILENAME
				, CASE
					WHEN NOT PICTREFUNITUNIT IS NULL THEN 0
					WHEN NOT PICTREFPRODKEYID IS NULL THEN 1
					WHEN NOT PICTREFITEMKEYID IS NULL THEN 2
					END AS SORTORDER
                , CASE
                    WHEN NOT PICTREFUNITUNIT IS NULL THEN @LinkTypeUnit
                    WHEN NOT PICTREFPRODKEYID IS NULL THEN @LinkTypeProduct
                    WHEN NOT PICTREFITEMKEYID IS NULL THEN @LinkTypeItem
                END AS LINKTYPE
            FROM
	            TPICTREF
            JOIN
	            TPICS
            ON
	            PICTPICSKEYID = PICSKEYID
            WHERE
                (
					PICTREFUNITUNIT = @UNITKEYID
					OR
					PICTREFPRODKEYID = @PRODUCTKEYID
					OR
					PICTREFITEMKEYID = @ITEMKEYID
				)
                AND
                PICTSERIES = @SeriesType
            ORDER BY
                SORTORDER, PICTNO
        ";

    #endregion SQL queries for media series

    private class MediaSeriesReferenceColumn
    {
        private const string Catalog = "PICTREFCATKEYID";
        private const string Item = "PICTREFITEMKEYID";
        private const string Machine = "PICTREFMACHKEYID";
        private const string Product = "PICTREFPRODKEYID";
        private const string Trigger = "PICTREFTRIGKEYID";
        private const string Unit = "PICTREFUNITUNIT";
        private const string User = "PICTREFUSERKEYID";

        /// <summary>
        /// Translates a link type into corresponding reference column name.
        /// </summary>
        /// <param name="linkType">
        /// The type of link; refer to <see cref="MediaSeriesLinks"/> for valid values.
        /// </param>
        /// <returns>The name of the reference column.</returns>
        public static string FromLinkType(string linkType)
        {
            return linkType.ToLower() switch
            {
                MediaSeriesLinks.Catalog => Catalog,
                MediaSeriesLinks.Item => Item,
                MediaSeriesLinks.Machine => Machine,
                MediaSeriesLinks.Product => Product,
                MediaSeriesLinks.Trigger => Trigger,
                MediaSeriesLinks.Unit => Unit,
                MediaSeriesLinks.User => User,
                _ => throw new NotSupportedException($"Link type \"{linkType}\" is not supported.")
            };
        }
    }

    private class MediaSeriesDto
    {
        public int PictNo { get; set; }

        public int PicsKeyId { get; set; }

        public bool HasImage { get; set; }

        public bool HasSound { get; set; }

        public bool HasText { get; set; }

        public bool HasThumbnail { get; set; }

        public bool HasVideo { get; set; }

        public string PicsOrgFilename { get; set; }

        public string LinkType { get; set; }
    }
}