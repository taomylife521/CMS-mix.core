﻿namespace Mix.Auth.Models
{
    public class OAuthRequest
    {
        public OAuthRequest() { }
        /// <summary>
        /// Response Type, is required
        /// </summary>
        public string response_type { get; set; }

        /// <summary>
        /// Client Id, is required
        /// </summary>

        public Guid client_id { get; set; }

        /// <summary>
        /// Redirect Uri, is optional
        /// The redirection endpoint URI MUST be an absolute URI as defined by
        /// [RFC3986] Section 4.3
        /// </summary>

        public string redirect_uri { get; set; }

        /// <summary>
        /// Optional
        /// </summary>
        public string scope { get; set; }

        /// <summary>
        /// Return the state in the result 
        /// if it was present in the client authorization request
        /// </summary>
        public string state { get; set; }

        /// <summary>
        /// This is generated by the client
        /// and the Authorization server must return it to client
        /// after generating the identity_token
        /// </summary>
        public string nonce { get; set; }

        /// <summary>
        /// if is not null so the client use Pkce
        /// </summary>
        public string code_challenge { get; set; }

        /// <summary>
        /// Hasher type for <see cref="code_challenge"/>
        /// </summary>
        public string code_challenge_method { get; set; }
    }
}
