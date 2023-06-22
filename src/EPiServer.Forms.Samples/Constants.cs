namespace EPiServer.Forms.Samples
{
    public class Constants
    {
        /// <summary>
        /// "EPiServer.Forms.Samples"
        /// </summary>
        public const string ModuleName = "EPiServer.Forms.Samples";
        public const int DefaultMapImageWidth = 300;
        public const int DefaultMapImageHeight = 150;

        /// <summary>
        /// Default score threshold of reCaptcha.
        /// </summary>
        public const double DefaultRecaptchaScoreThreshold = 0.5;

        /// <summary>
        /// Default score threshold of hCaptcha.
        /// </summary>
        public const double DefaultHcaptchaScoreThreshold = 0.5;

        /// <summary>
        /// Minimum score threshold of reCaptcha.
        /// </summary>
        public const double MinimumRecaptchaScoreThreshold = 0;

        /// <summary>
        /// Maximum score threshold of reCaptcha.
        /// </summary>
        public const double MaximumRecaptchaScoreThreshold = 1;
    }
}
