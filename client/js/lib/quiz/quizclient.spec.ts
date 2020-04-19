import * as nock from "nock";
import { QuizClient } from "./quizclient";
import {
    GetQuizResponse,
    QuizInfoDto,
    GetAllQuizzesResponse,
    CreateQuizRequest,
} from "./dto";

const baseUrl = "http://localhost";
const client = new QuizClient(baseUrl);
const token = "123";
const quizId = "123-456";
const quizName = "TestQuiz";

beforeEach(() => {
    if (!nock.isActive()) {
        nock.activate();
    }
});

afterEach(() => {
    nock.restore();
});

describe("quiz client", () => {
    it("gets single quiz", async () => {
        const quiz: GetQuizResponse = {
            quizName: quizName,
            quizId: quizId,
            tasks: [
                {
                    answer: "42",
                    question: "Meaning of life",
                    taskId: "123-456",
                    variants: ["Love", "Death", "42"],
                },
            ],
        };

        const scope = nock(baseUrl)
            .get(`/api/v1/quiz/${quizId}`)
            .matchHeader("Authorization", "Bearer " + token)
            .reply(200, JSON.stringify(quiz));

        const quizResponse = await client.getQuizAsync(quizId, token);

        expect(quizResponse.responseCode).toBe(200);
        expect(quizResponse.errorInfo).toBeUndefined();
        expect(quizResponse.responseData).toStrictEqual(
            GetQuizResponse.toModel(quiz)
        );

        scope.done();
    });

    it("gets all quizzes", async () => {
        const quizzes: GetAllQuizzesResponse = {
            quizzes: [
                {
                    quizName: quizName,
                    quizId: "123-456",
                },
                {
                    quizName: quizName,
                    quizId: "567-890",
                },
            ],
        };

        const scope = nock(baseUrl)
            .get("/api/v1/quiz/all")
            .matchHeader("Authorization", "Bearer " + token)
            .reply(200, JSON.stringify(quizzes));

        const quizzesResponse = await client.getAllQuizzesAsync(token);

        expect(quizzesResponse.responseCode).toBe(200);
        expect(quizzesResponse.errorInfo).toBeUndefined();
        expect(quizzesResponse.responseData).toStrictEqual(
            quizzes.quizzes.map((q) => QuizInfoDto.toInfo(q))
        );

        scope.done();
    });

    it("creates quiz", async () => {
        const request: CreateQuizRequest = {
            quizName: quizName,
            tasks: ["123-456", "456-789"],
        };

        const quizInfo: QuizInfoDto = {
            quizName: quizName,
            quizId: "123123",
        };

        const scope = nock(baseUrl)
            .post("/api/v1/quiz/new")
            .matchHeader("Authorization", "Bearer " + token)
            .reply(200, JSON.stringify(quizInfo));

        const newQuiz = await client.createQuizAsync(
            quizName,
            request.tasks,
            token
        );

        expect(newQuiz.responseCode).toBe(200);
        expect(newQuiz.errorInfo).toBeUndefined();
        expect(newQuiz.responseData).toStrictEqual(
            QuizInfoDto.toInfo(quizInfo)
        );

        scope.done();
    });
});
