import * as nock from "nock";
import { AuthClient } from "./authclient";
import { AuthToken } from "./models";

let client: AuthClient;
const baseUrl = "http://localhost";
const registrationUrl = "/auth/v1/register";

const email = "foo@bar.baz";
const password = "qwerty";
const token: AuthToken = "1234";

beforeAll(() => {
    client = new AuthClient(baseUrl);
});

afterEach(() => {
    nock.restore();
    nock.activate();
});

test("registers user", async () => {
    const scope = nock(baseUrl)
        .post(registrationUrl, password)
        .query({ email: email })
        .reply(200, token);

    const response = await client.registerAsync(email, password);

    expect(response.responseCode).toBe(200);
    expect(response.responseData).toBe(token);

    scope.done();
});

test("handles bad request on empty email", async () => {
    const emptyEmailError = "email can not be empty";
    const scope = nock(baseUrl)
        .post(registrationUrl, password)
        .query({ email: "" })
        .reply(400, emptyEmailError);

    const response = await client.registerAsync("", password);

    expect(response.responseCode).toBe(400);
    expect(response.responseData).toBeUndefined();
    expect(response.errorInfo).toBe(emptyEmailError);

    scope.done();
});

test("handles bad request on empty password", async () => {
    const emptyPasswordError = "password can not be empty";
    const scope = nock(baseUrl)
        .post(registrationUrl, "")
        .query({ email: email })
        .reply(400, emptyPasswordError);

    const response = await client.registerAsync(email, "");

    expect(response.responseCode).toBe(400);
    expect(response.responseData).toBeUndefined();
    expect(response.errorInfo).toBe(emptyPasswordError);

    scope.done();
});
