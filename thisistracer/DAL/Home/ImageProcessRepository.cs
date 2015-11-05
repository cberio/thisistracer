using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace thisistracer.DAL.Home {
    public class ImageProcessRepository : IImageProcessRepository {
        private Image img;
        private MemoryStream ms;

        public ImageProcessRepository() {
            
        }

        public Dictionary<string, string> GenerateMetadataFromImg(MemoryStream imgMs) {
            Dictionary<string, string> metadata = new Dictionary<string, string>();

            img = Image.FromStream(imgMs);
            var lat = GetLatitude(img);
            var lng = GetLongitude(img);
            var date = GetPropertyItemValue(img, 36867) ?? null; // pic date

            if (lat != null && lng != null) {
                metadata.Add("lat", lat.ToString());
                metadata.Add("lng", lng.ToString());
            }
            if (date != null) {
                Regex r = new Regex(":");
                metadata.Add("Date", r.Replace(Encoding.UTF8.GetString(date).Replace("\0", ""), "-", 2));
            }

            return metadata;
        }

        public float? GetLatitude(Image targetImg) {
            try {
                //Property Item 0x0001 - PropertyTagGpsLatitudeRef
                PropertyItem propItemRef = targetImg.GetPropertyItem(1);
                //Property Item 0x0002 - PropertyTagGpsLatitude
                PropertyItem propItemLat = targetImg.GetPropertyItem(2);

                //https://cberio.blob.core.windows.net/photo/collection01/20150723_101520.jpg
                //https://cberio.blob.core.windows.net/photo/collection01/20150722_234712.jpg

                //System.Diagnostics.Debug.WriteLine("==== Orient = " + targetImg.GetPropertyItem(274).Value[0]);
                return ExifGpsToFloat(propItemRef, propItemLat);
            } catch (ArgumentException) {
                return null;
            } finally {
                //if (targetImg != null)
                //    targetImg.Dispose();
            }
        }
        public float? GetLongitude(Image targetImg) {
            try {
                ///Property Item 0x0003 - PropertyTagGpsLongitudeRef
                PropertyItem propItemRef = targetImg.GetPropertyItem(0x0003);
                //Property Item 0x0004 - PropertyTagGpsLongitude
                PropertyItem propItemLong = targetImg.GetPropertyItem(0x0004);
                return ExifGpsToFloat(propItemRef, propItemLong);
            } catch (ArgumentException) {
                return null;
            } finally {
                //if (targetImg != null)
                //    targetImg.Dispose();
            }
        }

        public float ExifGpsToFloat(PropertyItem propItemRef, PropertyItem propItem) {
            uint degreesNumerator = BitConverter.ToUInt32(propItem.Value, 0);
            uint degreesDenominator = BitConverter.ToUInt32(propItem.Value, 4);
            float degrees = degreesNumerator / (float)degreesDenominator;

            uint minutesNumerator = BitConverter.ToUInt32(propItem.Value, 8);
            uint minutesDenominator = BitConverter.ToUInt32(propItem.Value, 12);
            float minutes = minutesNumerator / (float)minutesDenominator;

            uint secondsNumerator = BitConverter.ToUInt32(propItem.Value, 16);
            uint secondsDenominator = BitConverter.ToUInt32(propItem.Value, 20);
            float seconds = secondsNumerator / (float)secondsDenominator;

            float coorditate = degrees + (minutes / 60f) + (seconds / 3600f);
            string gpsRef = System.Text.Encoding.ASCII.GetString(new byte[1] { propItemRef.Value[0] }); //N, S, E, or W
            if (gpsRef == "S" || gpsRef == "W")
                coorditate = 0 - coorditate;
            return coorditate;
        }
        public byte[] GetPropertyItemValue(Image img, int propId) {
            byte[] propVal = null;

            try {
                PropertyItem propItem = img.GetPropertyItem(propId);
                propVal = propItem.Value;
            } catch (Exception ex) {
                throw new ArgumentNullException(ex.InnerException.ToString());
            }

            return propVal;
        }

        public async Task<Image> LoadPicture(Uri url) {
            HttpWebRequest wreq;
            HttpWebResponse wresp;
            Stream mystream;
            Bitmap bmp;

            bmp = null;
            mystream = null;
            wresp = null;
            try {
                wreq = (HttpWebRequest)WebRequest.Create(url);
                wreq.AllowWriteStreamBuffering = true;
                wresp = (HttpWebResponse)await Task.Factory
                    .FromAsync<WebResponse>(wreq.BeginGetResponse,
                                            wreq.EndGetResponse,
                                            null);

                if ((mystream = wresp.GetResponseStream()) != null)
                    bmp = new Bitmap(mystream);

                //wreq = (HttpWebRequest)HttpWebRequest.Create(url);
                //wreq.AllowWriteStreamBuffering = true;

                //wresp = (HttpWebResponse)wreq.GetResponse();

                //if ((mystream = wresp.GetResponseStream()) != null)
                //    bmp = new Bitmap(mystream);
            } catch {
                // Do nothing... 
            } finally {
                if (mystream != null)
                    mystream.Close();

                if (wresp != null)
                    wresp.Close();
            }

            return (Image)bmp;
        }
        public Image RotateImage(Image bmp) {
            if (Array.IndexOf(bmp.PropertyIdList, 274) > -1) {
                var orientation = (int)bmp.GetPropertyItem(274).Value[0];
                switch (orientation) {
                    case 1:
                        // No rotation required.
                        break;
                    case 2:
                        bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        break;
                    case 3:
                        bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case 4:
                        bmp.RotateFlip(RotateFlipType.Rotate180FlipX);
                        break;
                    case 5:
                        bmp.RotateFlip(RotateFlipType.Rotate90FlipX);
                        break;
                    case 6:
                        bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 7:
                        bmp.RotateFlip(RotateFlipType.Rotate270FlipX);
                        break;
                    case 8:
                        bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                }
                // This EXIF data is now invalid and should be removed.
                bmp.RemovePropertyItem(274);
            }

            return bmp;
        }

        public MemoryStream ProcessB4Upload(System.Web.HttpPostedFileBase fs) {
            ms = new MemoryStream();
            ms.Position = 0;

            img = Image.FromStream(fs.InputStream);
            img = RotateImage(img);
            img.Save(ms, ImageFormat.Jpeg);
            img.Dispose();

            return ms;
        }
    }
}
