using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// this class is created for "future proofing" the hashing the password , when computation gets faster we can increase the number of iterations for better security
/// </summary>
namespace IoT_SmartPlant_Portal.JwtAuth.Models {

    public sealed class HashingOptions {
        public int Iterations { get; set; } = 10000;
    }
}
