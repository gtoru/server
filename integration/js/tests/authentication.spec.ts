import {
    AuthClient,
    User,
    PersonalInfo,
    AuthToken,
} from "../../../client/js/lib/index";

function getClient(): AuthClient {
    return new AuthClient("http://localhost:8080");
}

describe("authentication", () => {
    const email = "foo@bar.baz";
    const password = "qwerty";

    const personalInfo: PersonalInfo = {
        address: "Bakers st.",
        birthday: new Date(1970, 1, 1),
        employer: "G-man",
        name: "John Doe",
        occupation: "Entertainer",
    };

    const user: User = {
        email: email,
        password: password,
        personalInfo: personalInfo,
    };

    it("creates user", async () => {
        const client = getClient();
        const userCreationResponse = await client.registerAsync(user, 3000);

        expect(userCreationResponse.responseCode).toBe(200);
        expect(userCreationResponse.errorInfo).toBeUndefined();
        expect(userCreationResponse.responseData).toBeUndefined();
    });

    let token: AuthToken;

    it("authenticates registered user", async () => {
        const client = getClient();
        const authenticationResponse = await client.authenticateAsync(
            email,
            password,
            3000
        );

        expect(authenticationResponse.responseCode).toBe(200);
        expect(authenticationResponse.errorInfo).toBeUndefined();
        expect(authenticationResponse.responseData).not.toBeUndefined();
        expect(authenticationResponse.responseData).not.toBeNull();

        token = authenticationResponse.responseData;
    });

    it("gets session info", async () => {
        const client = getClient();
        const sessionInfoResponse = await client.getSessionInfoAsync(
            token,
            3000
        );

        expect(sessionInfoResponse.responseCode).toBe(200);
        expect(sessionInfoResponse.errorInfo).toBeUndefined();
        expect(sessionInfoResponse.responseData.email).toBe(email);
        expect(sessionInfoResponse.responseData.personalInfo).toStrictEqual(
            personalInfo
        );
    });

    it("fails to create user with same email", async () => {
        const client = getClient();
        const registrationResponse = await client.registerAsync(user, 3000);

        expect(registrationResponse.responseCode).toBe(409);
    });

    it("fails to authenticate with wrong password", async () => {
        const client = getClient();
        const authenticationResponse = await client.authenticateAsync(
            email,
            "123",
            3000
        );

        expect(authenticationResponse.responseCode).toBe(403);
    });

    it("fails to authenticate withewrong email", async () => {
        const client = getClient();
        const authenticationResponse = await client.authenticateAsync(
            "foo",
            password,
            3000
        );

        expect(authenticationResponse.responseCode).toBe(403);
    });
});
