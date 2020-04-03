import * as nock from "nock";
import { AuthClient } from "./authclient";
import { AuthToken, User } from "./models";

let client: AuthClient;
const baseUrl = "http://localhost";
const registrationUrl = "/auth/v1/register";

const token: AuthToken = "1234";

const user: User = {
    address: "Moscow",
    birthday: new Date("1970-02-29"),
    email: "foo@bar.baz",
    employer: "Gazprom",
    name: "John Doe",
    occupation: "Worker",
    password: "qwerty",
};

beforeAll(() => {
    client = new AuthClient(baseUrl);
});

afterEach(() => {
    nock.restore();
    nock.activate();
});

test("registers user", async () => {
    const scope = nock(baseUrl)
        .post(registrationUrl, JSON.stringify(user))
        .reply(200, token);

    const response = await client.registerAsync(user);

    expect(response.responseCode).toBe(200);
    expect(response.responseData).toBe(token);

    scope.done();
});

test("handles bad request on empty user", async () => {
    const emptyPasswordError = "password can not be empty";
    const scope = nock(baseUrl)
        .post(registrationUrl, {})
        .reply(400, emptyPasswordError);

    const response = await client.registerAsync(new User());

    expect(response.responseCode).toBe(400);
    expect(response.responseData).toBeUndefined();
    expect(response.errorInfo).toBe(emptyPasswordError);

    scope.done();
});
