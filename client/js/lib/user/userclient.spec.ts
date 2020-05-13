import * as nock from "nock";
import { UserClient } from "./userclient";
import {
    TestSessionInfoResponse,
    NewSessionRequest,
    TestSessionResultsResponse,
    TestSessionResultResponse,
    AddAnswersRequest,
    UserCountResponse,
} from "./dto";

const baseUrl = "http://localhost";
const token = "1234";
const client = new UserClient(baseUrl);
const userId = "456-789";
const quizId = "1337";

const quizInfo: TestSessionInfoResponse = {
    tasks: [{ answers: ["123"], question: "42?" }],
    testSessionId: "123",
};

afterEach(() => {
    nock.restore();
    nock.activate();
});

describe("user client", () => {
    it("starts new session", async () => {
        const request: NewSessionRequest = {
            quizId: quizId,
        };

        const scope = nock(baseUrl)
            .post(
                `/api/v1/user/${userId}/sessions/new`,
                JSON.stringify(request)
            )
            .matchHeader("Authorization", `Bearer ${token}`)
            .reply(200, JSON.stringify(quizInfo));

        const sessionCreation = await client.startNewSessionAsync(
            userId,
            quizId,
            token
        );

        expect(sessionCreation.responseCode).toBe(200);
        expect(sessionCreation.errorInfo).toBeUndefined();
        expect(sessionCreation.responseData.tasks).toStrictEqual(
            quizInfo.tasks
        );
        expect(sessionCreation.responseData.testSessionId).toBe(
            quizInfo.testSessionId
        );

        scope.done();
    });

    it("gets current test session", async () => {
        const scope = nock(baseUrl)
            .get(`/api/v1/user/${userId}/sessions/current`)
            .matchHeader("Authorization", `Bearer ${token}`)
            .reply(200, JSON.stringify(quizInfo));

        const sessionInfo = await client.getCurrentTestSessionAsync(
            userId,
            token
        );

        expect(sessionInfo.responseCode).toBe(200);
        expect(sessionInfo.errorInfo).toBeUndefined();
        expect(sessionInfo.responseData.tasks).toStrictEqual(quizInfo.tasks);
        expect(sessionInfo.responseData.testSessionId).toBe(
            quizInfo.testSessionId
        );

        scope.done();
    });

    it("gets test session results", async () => {
        const response: TestSessionResultsResponse = {
            results: [
                {
                    result: 10,
                    testSessionId: quizInfo.testSessionId,
                },
            ],
        };

        const scope = nock(baseUrl)
            .get(`/api/v1/user/${userId}/sessions/results`)
            .matchHeader("Authorization", `Bearer ${token}`)
            .reply(200, JSON.stringify(response));

        const result = await client.getResultsAsync(userId, token);

        expect(result.responseCode).toBe(200);
        expect(result.errorInfo).toBeUndefined();
        expect(result.responseData).toHaveLength(1);
        expect(result.responseData[0].result).toBe(response.results[0].result);
        expect(result.responseData[0].testSessionId).toBe(
            response.results[0].testSessionId
        );

        scope.done();
    });

    it("ends current session", async () => {
        const response: TestSessionResultResponse = {
            result: 10,
            testSessionId: quizInfo.testSessionId,
        };

        const scope = nock(baseUrl)
            .post(`/api/v1/user/${userId}/sessions/end`)
            .matchHeader("Authorization", `Bearer ${token}`)
            .reply(200, JSON.stringify(response));

        const sessionResult = await client.endSessionAsync(userId, token);

        expect(sessionResult.responseCode).toBe(200);
        expect(sessionResult.errorInfo).toBeUndefined();
        expect(sessionResult.responseData.result).toBe(response.result);
        expect(sessionResult.responseData.testSessionId).toBe(
            response.testSessionId
        );

        scope.done();
    });

    it("adds answers", async () => {
        const request: AddAnswersRequest = {
            answers: [
                {
                    answer: "42",
                    taskNumber: 0,
                },
            ],
        };

        const scope = nock(baseUrl)
            .post(
                `/api/v1/user/${userId}/sessions/answer`,
                JSON.stringify(request)
            )
            .matchHeader("Authorization", `Bearer ${token}`)
            .reply(200);

        const answerAdd = await client.addAnswerAsync(
            userId,
            [{ answer: "42", taskNumber: 0 }],
            token
        );

        expect(answerAdd.responseCode).toBe(200);
        expect(answerAdd.errorInfo).toBeUndefined();
        expect(answerAdd.responseData).toBeUndefined();

        scope.done();
    });

    it("gets user count", async () => {
        const response: UserCountResponse = {
            userCount: 42,
        };

        const scope = nock(baseUrl)
            .get("/api/v1/user/count")
            .matchHeader("Authorization", `Bearer ${token}`)
            .reply(200, JSON.stringify(response));

        const userCountResponse = await client.getUserCountAsync(token);

        expect(userCountResponse.errorInfo).toBeUndefined();
        expect(userCountResponse.responseCode).toBe(200);
        expect(userCountResponse.responseData).toBe(42);

        scope.done();
    });
});
