using System;
using System.Collections.Generic;
using System.Linq;

namespace CatsCloset.Apis {
	public static class Magic {
		private static readonly Dictionary<byte[], string> Database = new Dictionary<byte[], string>() {
			{ new byte[] { 0x42, 0x4d             }, "image/bmp"  },
			{ new byte[] { 0x47, 0x49, 0x46, 0x38 }, "image/gif"  },
			{ new byte[] { 0xff, 0xd8, 0xff, 0xe0 }, "image/jpeg" },
			{ new byte[] { 0x89, 0x50, 0x4e, 0x47 }, "image/png"  }
		};
		private const string DefaultMime = "application/octet-stream";

		private static bool MatchesMagic(byte[] head, byte[] magic) {
			if ( head.Length < magic.Length ) {
				return false;
			}
			for ( int i = 0; i < magic.Length; ++i ) {
				if ( head[i] != magic[i] ) {
					return false;
				}
			}
			return true;
		}

		public static string GetMime(byte[] head) {
			KeyValuePair<byte[], string> mime = Database.FirstOrDefault(
				p => MatchesMagic(head, p.Key));
			return mime.Value == null ? DefaultMime : mime.Value;
		}
	}
}

