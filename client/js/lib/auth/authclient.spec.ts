import * as nock from "nock";
import { AuthClient } from "./authclient";
import { AuthToken, User, SessionInfo, PersonalInfo } from "./models";
import { UserDto, SessionInfoDto } from "./dto";

let client: AuthClient;
const baseUrl = "http://localhost";

const registrationUrl = "/api/auth/v1/user/register";
const authenticationUrl = "/api/auth/v1/user/authenticate";
const sessionInfoUrl = "/api/auth/v1/sessions/my";

const token: AuthToken = "1234";

const personalInfo: PersonalInfo = {
    address: "Moscow",
    birthday: new Date("1970-02-29"),
    employer: "Gazprom",
    name: "John Doe",
    occupation: "Worker",
};

const user: User = {
    email: "foo@bar.baz",
    password: "qwerty",
    personalInfo: personalInfo,
};

const sessionInfo: SessionInfo = {
    email: user.email,
    userId: "1234",
    personalInfo: personalInfo,
};

beforeAll(() => {
    client = new AuthClient(baseUrl);
});

afterEach(() => {
    nock.restore();
    nock.activate();
});

describe("registration", () => {
    it("registers user", async () => {
        const scope = nock(baseUrl)
            .post(registrationUrl, JSON.stringify(UserDto.fromModel(user)))
            .reply(200, token);

        const response = await client.registerAsync(user);

        expect(response.responseCode).toBe(200);
        expect(response.responseData).toBeUndefined();

        scope.done();
    });

    it("handles bad request on empty user", async () => {
        const emptyPasswordError = "password can not be empty";
        const scope = nock(baseUrl)
            .post(
                registrationUrl,
                JSON.stringify(UserDto.fromModel(new User()))
            )
            .reply(400, emptyPasswordError);

        const response = await client.registerAsync(new User());

        expect(response.responseCode).toBe(400);
        expect(response.responseData).toBeUndefined();
        expect(response.errorInfo).toBe(emptyPasswordError);

        scope.done();
    });
});

describe("authentication", () => {
    it("authenticates user", async () => {
        const scope = nock(baseUrl)
            .post(
                authenticationUrl,
                JSON.stringify({
                    email: user.email,
                    password: user.password,
                })
            )
            .reply(200, token);

        const response = await client.authenticateAsync(
            user.email,
            user.password
        );

        expect(response.responseCode).toBe(200);
        expect(response.responseData).toBe(token);

        scope.done();
    });

    it("should handle authentication error", async () => {
        const badAuth = "bad password or login";
        const scope = nock(baseUrl)
            .post(
                authenticationUrl,
                JSON.stringify({
                    email: user.email,
                    password: "",
                })
            )
            .reply(401, badAuth);

        const response = await client.authenticateAsync(user.email, "");

        expect(response.responseCode).toBe(401);
        expect(response.errorInfo).toBe(badAuth);
        expect(response.responseData).toBeUndefined();

        scope.done();
    });
});

describe("getting of session info", () => {
    it("should parse session info response", async () => {
        const authToken: AuthToken = "qwerty123";
        const scope = nock(baseUrl)
            .get(sessionInfoUrl)
            .matchHeader("Authorization", "Bearer " + authToken)
            .reply(200, JSON.stringify(SessionInfoDto.fromModel(sessionInfo)));

        const response = await client.getSessionInfoAsync(authToken);

        expect(response.responseCode).toBe(200);
        expect(response.errorInfo).toBeUndefined();
        expect(response.responseData).toStrictEqual(sessionInfo);

        scope.done();
    });

    it("should parse error response", async () => {
        const authToken = "123";
        const scope = nock(baseUrl)
            .get(sessionInfoUrl)
            .matchHeader("Authorization", "Bearer " + authToken)
            .reply(401, "unauthorized");

        const response = await client.getSessionInfoAsync(authToken);

        expect(response.responseCode).toBe(401);
        expect(response.errorInfo).toBe("unauthorized");
        expect(response.responseData).toBeUndefined();

        scope.done();
    });
});
