﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.PdfFonts;
using Xunit;

namespace Tabula.Tests
{
    public static class UtilsForTesting
    {
        public static readonly FontDetails HELVETICA_BOLD = new FontDetails("HELVETICA_BOLD", true, 0, false);

        public static PageArea getAreaFromFirstPage(string path, PdfRectangle pdfRectangle)
        {
            return getAreaFromPage(path, 1, pdfRectangle);
        }

        public static PageArea getAreaFromPage(string path, int page, PdfRectangle pdfRectangle)
        {
            var area = getPage(path, page);

            return area.getArea(pdfRectangle);
        }

        public static PageArea getPage(string path, int pageNumber)
        {
            ObjectExtractor oe = null;
            try
            {
                PageArea page;
                using (PdfDocument document = PdfDocument.Open(path, new ParsingOptions() { ClipPaths = true }))
                {
                    oe = new ObjectExtractor(document);
                    page = oe.extract(pageNumber);
                }
                return page;
            }
            finally
            {
                if (oe != null) oe.close();
            }
        }

        public static string[][] tableToArrayOfRows(Table table)
        {
            var tableRows = table.getRows();

            int maxColCount = 0;

            for (int i = 0; i < tableRows.Count; i++)
            {
                var row = tableRows[i];
                if (maxColCount < row.Count)
                {
                    maxColCount = row.Count;
                }
            }

            Assert.Equal(maxColCount, table.getColCount());

            string[][] rv = new string[tableRows.Count][];

            for (int i = 0; i < tableRows.Count; i++)
            {
                var row = tableRows[i];
                rv[i] = new string[maxColCount];
                for (int j = 0; j < row.Count; j++)
                {
                    rv[i][j] = table.getCell(i, j).getText();
                }
            }

            return rv;
        }

        public static string loadJson(string path)
        {
            return File.ReadAllText(path, Encoding.UTF8);

            /*
            StringBuilder stringBuilder = new StringBuilder();

            using (BufferedReader reader = new BufferedReader(new InputStreamReader(new FileInputStream(path), "UTF-8")))
            {
                String line = null;
                while ((line = reader.readLine()) != null)
                {
                    stringBuilder.Append(line);
                }
            }

            return stringBuilder.ToString();
            */
        }

        public static string loadCsv(string path)
        {
            return File.ReadAllText(path, Encoding.UTF8).Replace("(?<!\r)\n", "\r");

            /*
            StringBuilder outp = new StringBuilder();
            CSVParser parse = org.apache.commons.csv.CSVParser.parse(new File(path), Charset.forName("utf-8"), CSVFormat.EXCEL);

            CSVPrinter printer = new CSVPrinter(out, CSVFormat.EXCEL);
            printer.printRecords(parse);
            printer.close();

            String csv = outp.ToString().replaceAll("(?<!\r)\n", "\r");
            return csv;
            */
        }
    }
}
