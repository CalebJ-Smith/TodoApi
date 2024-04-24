# TodoApi
I used Visual Studio 2022 to develop, so proof-of-concept testing was easily done by running the project and using the "try it out" feature from the swagger/OpenAPI documentation page.

## Things I would change in a real-world implementation
- Test coverage. I've got proof-of-concept unit tests going, but wildly incomplete.
  - Since I'm dealing with locking, it would also be good to have some stress tests to validate that.
  - More manual testing as a proof-to-myself
- Since I'm using cookies from `HttpContext.Session' to authorize users, encrypt and sign them. There's very likely an existing library/assembly for this, I just didn't look hard enough.  
  - Overall, my handling of this should be wrapped up in routing middleware, or at the very least a decorator/attribute
  - I didn't figure out how to change users in swagger; I expected to be able to modify a cookie element in the curl request but that wasn't presented.

## Decisions I'm a little unsure of
- If I should have the `InMemoryTodoListRepository` implementation combined with `InMemoryTodoListItemRepository`. It's been a while since I last used Entity Framework, and I forgot that specific practice.
  - It's possible that I would be able to make more use of the fine-grained locking provided by `TodoListWrapper.Lock` if I did.
- Registering my Repository classes as singletons instead of as scoped and using a singleton backing store to enable the repository to remain Scoped.
  - I aimed to make the persistance layer easily swappable with a more permanent provider, but assumed that would be done by replacing my Repository classes with Repositories that wrapped EF or another ORM as that was what my previous industry experience with .NET used. However, the in-memory persistance could be replaced with a hard disk that exposed an `IDictionary` interface
 
