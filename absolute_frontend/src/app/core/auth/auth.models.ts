export type LoginRequestDto = {
    userNameOrEmail: string;
    password: string;
};
// export type RegisterRequestDto = {
//   userName: string;
//   email: string;
//   fullName: string;
//   password: string;
// };

export type AuthResponseDto = {
    accessToken: string;
    accessTokenExpiresAtUtc: string;
    refreshTokenExpiresAtUtc: string;
};