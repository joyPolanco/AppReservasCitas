using Aplicacion.Casosdeuso;
using Aplicacion.DTOs;
using Aplicacion.Interfaces;
using Dominio.Entidades;
using Dominio.Entidades.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Servicios
{
    public class JwtTokenGenerator : IJwtTokenGenerator
       
    {
        JwtConfiguration _configuration;
        public JwtTokenGenerator (JwtConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateAccessToken(string email, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JwtConfiguration.Secret);
            
            Claim[] claims =
            {
              new Claim (ClaimTypes.Email,email),
              new Claim (ClaimTypes.Role ,role)
            };
            Console.WriteLine(claims);
            var tokenDescriptor = new SecurityTokenDescriptor
            { 
                NotBefore= DateTime.UtcNow,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_configuration.ExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
      
      

    }
}
